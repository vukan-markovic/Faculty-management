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
using Student_URIS.AuthFilters;
using Student_URIS.Models;
using URIS_v10.Filters;

namespace Student_URIS.Controllers
{
    [RoutePrefix("api-student/test")]
    public class TestController : ApiController
    {
        public string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

        // GET: api/Test
        [Route("")]
        [HttpGet]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Predavac)]
        public HttpResponseMessage GetTestovi()
        {
            List<Test> result = new List<Test>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Student.Test", connection)
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
                            var test = new Test
                            {
                                TestID = Convert.ToInt32(dataReader["TestID"]),
                                KursID = Convert.ToInt32(dataReader["KursID"]),
                                NazivTesta = Convert.ToString(dataReader["NazivTesta"])
                            };

                            result.Add(test);
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

        // GET: api/Test/5
        [HttpGet, Route("{id}")]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public async Task<HttpResponseMessage> GetByIDAsync(int id)
        {
            TestWithValueObject test = null;

            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Student.Test where TestID=" + id, connection)
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
                            test = new TestWithValueObject
                            {
                                TestID = Convert.ToInt32(dataReader["TestID"]),
                                KursID = Convert.ToInt32(dataReader["KursID"]),
                                NazivTesta = Convert.ToString(dataReader["NazivTesta"])
                            };
                        }
                    }

                    if (test == null)
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Error 404 Found");
                }
                catch (Exception )
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error retrieving data");
                }
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:10682/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("api-kurs/kurs/" + test.KursID);

                if (response.IsSuccessStatusCode)
                {
                    var Kurs = await response.Content.ReadAsAsync<KursVO>();
                    test.Kurs = Kurs;
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, test);
        }

        // POST: api/Test
        [HttpPost, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Predavac)]
        public HttpResponseMessage CreateTest([FromBody]Test test)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("INSERT INTO Student.Test VALUES(@NazivTesta, @KursID)", connection);
                sqlCmd.Parameters.AddWithValue("NazivTesta", test.NazivTesta);
                sqlCmd.Parameters.AddWithValue("KursID", test.KursID);

                try
                {
                    connection.Open();
                    int i = sqlCmd.ExecuteNonQuery();
                    SqlCommand sqlTest = new SqlCommand("SELECT TOP 1 * FROM Student.Test ORDER BY TestID DESC", connection);
                    Test last = new Test();
                    using (SqlDataReader testRead = sqlTest.ExecuteReader())
                    {
                        while (testRead.Read())
                        {
                            last.TestID = Convert.ToInt32(testRead["TestID"]);
                            last.KursID = Convert.ToInt32(testRead["KursID"]);
                            last.NazivTesta = Convert.ToString(testRead["NazivTesta"]);
                        }
                    }

                    var response = Request.CreateResponse(HttpStatusCode.Created, last);
                    response.Headers.Location = new Uri(Request.RequestUri + "/" + last.TestID);
                    return response;
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error 404 Bad request");
                }
            }
        }

        // PUT: api/Test/5
        [HttpPut, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Predavac)]
        public HttpResponseMessage UpdateTest([FromBody]Test test)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("UPDATE Student.Test SET KursID=@KursID, NazivTesta=@NazivTesta WHERE TestID=@id"))
                    {
                        connection.Open();
                        sqlCmd.Connection = connection;
                        sqlCmd.Parameters.AddWithValue("KursID", test.KursID);
                        sqlCmd.Parameters.AddWithValue("NazivTesta", test.NazivTesta);
                        sqlCmd.Parameters.AddWithValue("id", test.TestID);
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

        // DELETE: api/Test/5
        [HttpDelete, Route("{id}")]
        [Authorize(Roles = RolesConst.ROLE_Admin_Predavac)]
        public HttpResponseMessage DeleteTest(int id)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("DELETE FROM Student.Test where TestID=@id"))
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
