using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using Tim_URIS.AuthFilters;
using Tim_URIS.Models;
using URIS_v10.Filters;

namespace Tim_URIS.Controllers
{
    [RoutePrefix("api-tim/postavljeni-fajlovi")]
    public class PostavljeniFajloviController : ApiController
    {
        public string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

        // GET: api/PostavljeniFajlovi
        [Route("")]
        [HttpGet]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        [Authorize(Roles = RolesConst.ROLE_Admin)]
        public HttpResponseMessage GetPostavljeniFajlovi()
        {
            List<PostavljeniFajlovi> result = new List<PostavljeniFajlovi>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Tim.PostavljeniFajlovi", connection)
                {
                    CommandType = CommandType.Text
                };
                try
                {
                    connection.Open();
                    using (SqlDataReader dataReader = sqlCmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            var postavljeniFajlovi = new PostavljeniFajlovi();

                            postavljeniFajlovi.PostavljeniFajloviID = Convert.ToInt32(dataReader["PostavljeniFajloviID"]);
                            postavljeniFajlovi.NazivPostavljenogFajla = Convert.ToString(dataReader["NazivPostavljenogFajla"]);

                            if (!DBNull.Value.Equals(dataReader["StudentID"]))
                                postavljeniFajlovi.StudentID = Convert.ToInt32(dataReader["StudentID"]);

                            if (!DBNull.Value.Equals(dataReader["TimID"]))
                                postavljeniFajlovi.TimID = Convert.ToInt32(dataReader["TimID"]);
                            
                            result.Add(postavljeniFajlovi);
                        }
                    }
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error retrieving data");
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        // GET: api/PostavljeniFajlovi/5
        [HttpGet, Route("{id}")]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public async Task<HttpResponseMessage> GetByIDAsync(int id)
        {
            PostavljeniFajloviWithValueObject postavljeniFajlovi = null;
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Tim.PostavljeniFajlovi where PostavljeniFajloviID=" + id, connection)
                {
                    CommandType = CommandType.Text
                };
                try
                {
                    connection.Open();
                    using (SqlDataReader dataReader = sqlCmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            postavljeniFajlovi = new PostavljeniFajloviWithValueObject();

                            postavljeniFajlovi.PostavljeniFajloviID = Convert.ToInt32(dataReader["PostavljeniFajloviID"]);
                            postavljeniFajlovi.NazivPostavljenogFajla = Convert.ToString(dataReader["NazivPostavljenogFajla"]);

                            if (!DBNull.Value.Equals(dataReader["StudentID"]))
                                postavljeniFajlovi.StudentID = Convert.ToInt32(dataReader["StudentID"]);

                            if (!DBNull.Value.Equals(dataReader["TimID"]))
                                postavljeniFajlovi.TimID = Convert.ToInt32(dataReader["TimID"]);
                        }
                    }

                    if (postavljeniFajlovi == null)
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Error 404 Found");
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error retrieving data");
                }

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:63284/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = await client.GetAsync("api-student/student/" + postavljeniFajlovi.StudentID);

                    if (response.IsSuccessStatusCode)
                    {
                        var student = await response.Content.ReadAsAsync<StudentInfoVO>();
                        postavljeniFajlovi.Student = student;
                    }
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, postavljeniFajlovi);
        }

        // POST: api/PostavljeniFajlovi
        [HttpPost, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public HttpResponseMessage CreatePostavljenFajl([FromBody]PostavljeniFajlovi postavljeniFajlovi)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("INSERT INTO Tim.PostavljeniFajlovi VALUES(@NazivPostavljenogFajla, @StudentID, @TimID)", connection);
                sqlCmd.Parameters.AddWithValue("NazivPostavljenogFajla", postavljeniFajlovi.NazivPostavljenogFajla);
                sqlCmd.Parameters.AddWithValue("StudentID", postavljeniFajlovi.StudentID);
                sqlCmd.Parameters.AddWithValue("TimID", postavljeniFajlovi.TimID);

                try
                {
                    connection.Open();
                    int i = sqlCmd.ExecuteNonQuery();
                    SqlCommand sqlPostavljeniFajlovi = new SqlCommand("SELECT TOP 1 * FROM Tim.PostavljeniFajlovi ORDER BY PostavljeniFajloviID DESC", connection);
                    PostavljeniFajlovi last = new PostavljeniFajlovi();

                    using (SqlDataReader postavljeniFajloviRead = sqlPostavljeniFajlovi.ExecuteReader())
                    {
                        while (postavljeniFajloviRead.Read())
                        {
                            last.PostavljeniFajloviID = Convert.ToInt32(postavljeniFajloviRead["PostavljeniFajloviID"]);
                            last.NazivPostavljenogFajla = Convert.ToString(postavljeniFajloviRead["NazivPostavljenogFajla"]);

                            if (!DBNull.Value.Equals(postavljeniFajloviRead["StudentID"]))
                                last.StudentID = Convert.ToInt32(postavljeniFajloviRead["StudentID"]);

                            if (!DBNull.Value.Equals(postavljeniFajloviRead["TimID"]))
                                last.TimID = Convert.ToInt32(postavljeniFajloviRead["TimID"]);
                        }
                    }

                    var response = Request.CreateResponse(HttpStatusCode.Created, last);
                    response.Headers.Location = new Uri(Request.RequestUri + "/" + last.PostavljeniFajloviID);
                    return response;
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error 404 Bad request");
                }
            }
        }

        // PUT: api/PostavljeniFajlovi/5
        [HttpPut, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public HttpResponseMessage UpdatePostavljeniFajlovi([FromBody]PostavljeniFajlovi postavljeniFajlovi)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("UPDATE Tim.PostavljeniFajlovi SET NazivPostavljenogFajla=@NazivPostavljenogFajla, StudentID=@StudentID, TimID=@TimID WHERE PostavljeniFajloviID=@id"))
                    {
                        connection.Open();
                        sqlCmd.Connection = connection;
                        sqlCmd.Parameters.AddWithValue("NazivPostavljenogFajla", postavljeniFajlovi.NazivPostavljenogFajla);
                        sqlCmd.Parameters.AddWithValue("StudentID", postavljeniFajlovi.StudentID);
                        sqlCmd.Parameters.AddWithValue("TimID", postavljeniFajlovi.TimID);
                        sqlCmd.Parameters.AddWithValue("id", postavljeniFajlovi.PostavljeniFajloviID);
                        int rowAffected = sqlCmd.ExecuteNonQuery();
                        if (rowAffected == 0)
                            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Error 404 Not Found");
                    }
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error 400 Bad Request");
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, "Updated Successfully");
        }

        // DELETE: api/PostavljeniFajlovi/5
        [HttpDelete, Route("{id}")]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public HttpResponseMessage DeletePostavljeniFajlovi(int id)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("DELETE FROM Tim.PostavljeniFajlovi where PostavljeniFajloviID=@id"))
                    {
                        connection.Open();
                        sqlCmd.Connection = connection;
                        sqlCmd.Parameters.AddWithValue("id", id);
                        int rowAffected = sqlCmd.ExecuteNonQuery();
                        if (rowAffected == 0)
                            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Error 404 Not Found");
                    }
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error 404 Bad request");
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, "Deleted Successfully");
        }
    }
}
