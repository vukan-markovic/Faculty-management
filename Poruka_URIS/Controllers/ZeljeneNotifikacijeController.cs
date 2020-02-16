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
using Poruka_URIS.AuthFilters;
using Poruka_URIS.Models;
using URIS_v10.Filters;

namespace Poruka_URIS.Controllers
{
    [RoutePrefix("api-poruka/zeljene-notifikacije")]
    public class ZeljeneNotifikacijeController : ApiController
    {
        public string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

        // GET: api/ZeljeneNotifikacije
        [Route("")]
        [HttpGet]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        [Authorize(Roles = RolesConst.ROLE_Admin)]
        public HttpResponseMessage GetZeljeneNotifikacije()
        {
            List<ZeljeneNotifikacije> result = new List<ZeljeneNotifikacije>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Poruka.ZeljeneNotifikacije", connection)
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
                            var zeljeneNotifikacije = new ZeljeneNotifikacije
                            {
                                ZeljeneNotifikacijeID = Convert.ToInt32(dataReader["ZeljeneNotifikacijeID"]),
                                NotifikacijaID = Convert.ToInt32(dataReader["NotifikacijaID"]),
                                StudentID = Convert.ToInt32(dataReader["StudentID"])
                            };

                            result.Add(zeljeneNotifikacije);
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

        // GET: api/ZeljeneNotifikacije/5
        [HttpGet, Route("{id}")]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public async Task<HttpResponseMessage> GetByIDAsync(int id)
        {
            ZeljeneNotifikacijeWithValueObject zeljeneNotifikacije = null;
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Poruka.ZeljeneNotifikacije where ZeljeneNotifikacijeID=" + id, connection)
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
                            zeljeneNotifikacije = new ZeljeneNotifikacijeWithValueObject
                            {
                                ZeljeneNotifikacijeID = Convert.ToInt32(dataReader["ZeljeneNotifikacijeID"]),
                                NotifikacijaID = Convert.ToInt32(dataReader["NotifikacijaID"]),
                                StudentID = Convert.ToInt32(dataReader["StudentID"])
                            };
                        }
                    }

                    if (zeljeneNotifikacije == null)
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Error 404 Found");
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error retrieving data");
                }
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:63284/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("api-student/student/" + zeljeneNotifikacije.StudentID);

                if (response.IsSuccessStatusCode)
                {
                    var student = await response.Content.ReadAsAsync<NotifikovanStudentVO>();
                    zeljeneNotifikacije.Student = student;
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, zeljeneNotifikacije);
        }

        // POST: api/ZeljeneNotifikacije
        [HttpPost, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public HttpResponseMessage CreateZeljeneNotifikacije([FromBody]ZeljeneNotifikacije zeljeneNotifikacije)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("INSERT INTO Poruka.ZeljeneNotifikacije VALUES(@StudentID, @NotifikacijaID)", connection);
                sqlCmd.Parameters.AddWithValue("StudentID", zeljeneNotifikacije.StudentID);
                sqlCmd.Parameters.AddWithValue("NotifikacijaID", zeljeneNotifikacije.NotifikacijaID);

                try
                {
                    connection.Open();
                    int i = sqlCmd.ExecuteNonQuery();
                    SqlCommand sqlZeljeneNotifikacije = new SqlCommand("SELECT TOP 1 * FROM Poruka.ZeljeneNotifikacije ORDER BY ZeljeneNotifikacijeID DESC", connection);
                    ZeljeneNotifikacije last = new ZeljeneNotifikacije();
                    using (SqlDataReader zeljeneNotifikacijeRead = sqlZeljeneNotifikacije.ExecuteReader())
                    {
                        while (zeljeneNotifikacijeRead.Read())
                        {
                            last.ZeljeneNotifikacijeID = Convert.ToInt32(zeljeneNotifikacijeRead["ZeljeneNotifikacijeID"]);
                            last.NotifikacijaID = Convert.ToInt32(zeljeneNotifikacijeRead["NotifikacijaID"]);
                            last.StudentID = Convert.ToInt32(zeljeneNotifikacijeRead["StudentID"]);
                        }
                    }

                    var response = Request.CreateResponse(HttpStatusCode.Created, last);
                    response.Headers.Location = new Uri(Request.RequestUri + "/" + last.ZeljeneNotifikacijeID);
                    return response;
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error 404 Bad request");
                }
            }
        }

        // PUT: api/ZeljeneNotifikacije/5
        [HttpPut, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public HttpResponseMessage UpdateZeljeneNotifikacije([FromBody]ZeljeneNotifikacije zeljeneNotifikacije)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("UPDATE Poruka.ZeljeneNotifikacije SET StudentID=@StudentID, NotifikacijaID=@NotifikacijaID WHERE ZeljeneNotifikacijeID=@id"))
                    {
                        connection.Open();
                        sqlCmd.Connection = connection;
                        sqlCmd.Parameters.AddWithValue("StudentID", zeljeneNotifikacije.StudentID);
                        sqlCmd.Parameters.AddWithValue("NotifikacijaID", zeljeneNotifikacije.NotifikacijaID);
                        sqlCmd.Parameters.AddWithValue("id", zeljeneNotifikacije.ZeljeneNotifikacijeID);
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

        // DELETE: api/ZeljeneNotifikacije/5
        [HttpDelete, Route("{id}")]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public HttpResponseMessage DeleteZeljeneNotifikacije(int id)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("DELETE FROM Poruka.ZeljeneNotifikacije where ZeljeneNotifikacijeID=@id"))
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
