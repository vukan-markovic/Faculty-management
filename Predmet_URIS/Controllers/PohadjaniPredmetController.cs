using Predmet_URIS.AuthFilters;
using Predmet_URIS.Filters;
using Predmet_URIS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace Predmet_URIS.Controllers
{
    [RoutePrefix("api-predmet/pohadjanipredmet")]
    public class PohadjaniPredmetController : ApiController
    {
        public string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

        // GET: api/Predmet
        [Route("")]
        [HttpGet]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public HttpResponseMessage GetPohadjaniPredmeti()
        {
            List<PohadjaniPredmet> result = new List<PohadjaniPredmet>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Predmet.PohadjaniPredmet", connection)
                {
                    CommandType = CommandType.Text
                };
                try
                {
                    connection.Open();
                    using (var dataReader = sqlCmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            var pohadjaniPredmet = new PohadjaniPredmet
                            {
                                PohadjaniPredmetID = Convert.ToInt32(dataReader["PohadjaniPredmetID"]),
                                PredmetID = Convert.ToInt32(dataReader["PredmetID"]),
                                Ocena = Convert.ToInt32(dataReader["Ocena"]),
                                BrojBodova = Convert.ToInt32(dataReader["BrojBodova"]),
                                StudentID=Convert.ToInt32(dataReader["StudentID"])
                            };
                            result.Add(pohadjaniPredmet);
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

        
        // GET: api/PohadjaniPredmet/PredmetID
        [HttpGet, Route("{id}")]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public async Task<HttpResponseMessage> GetStudentiNaPredmetuAsync(int id)
        {
            List<PohadjaniPredmetWithValueObject> pohadjaniPredmetList = new List<PohadjaniPredmetWithValueObject>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Predmet.PohadjaniPredmet where PredmetID=" + id, connection)
                {
                    CommandType = CommandType.Text
                };
                try
                {
                    connection.Open();
                    using (var dataReader = sqlCmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            var pohadjaniPredmet = new PohadjaniPredmetWithValueObject
                            {
                                PohadjaniPredmetID = Convert.ToInt32(dataReader["PohadjaniPredmetID"]),
                                PredmetID = Convert.ToInt32(dataReader["PredmetID"]),
                                Ocena = Convert.ToInt32(dataReader["Ocena"]),
                                BrojBodova = Convert.ToInt32(dataReader["BrojBodova"]),
                                StudentID = Convert.ToInt32(dataReader["StudentID"])
                            };
                            using (var client = new HttpClient())
                            {
                                client.BaseAddress = new Uri("http://localhost:63284/");
                                client.DefaultRequestHeaders.Accept.Clear();
                                var authorization = Request.Headers.GetValues("Authorization").FirstOrDefault();
                                var token = authorization.Split(null)[1];
                                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                using (var response = await client.GetAsync("api-student/student/" + pohadjaniPredmet.StudentID))
                                {
                                    if (response.IsSuccessStatusCode)
                                    {
                                        var Student = await response.Content.ReadAsAsync<StudentNaPredmetuVO>();
                                        pohadjaniPredmet.Student = Student;
                                    }
                                }
                            }
                            pohadjaniPredmetList.Add(pohadjaniPredmet);
                        }
                    }
                    if (pohadjaniPredmetList == null)
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Error 404 Found");
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error retrieving data");
                }
                
            }
            return Request.CreateResponse(HttpStatusCode.OK, pohadjaniPredmetList);
        }

        // POST: api/PohadjaniPredmet
        [HttpPost, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public HttpResponseMessage CreatePohadjaniPredmet([FromBody]PohadjaniPredmet pohadjaniPredmet)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("INSERT INTO Predmet.PohadjaniPredmet VALUES(@PredmetID,@Ocena,@BrojBodova,@StudentID)", connection);
                sqlCmd.Parameters.AddWithValue("PredmetID", pohadjaniPredmet.PredmetID);
                sqlCmd.Parameters.AddWithValue("Ocena", pohadjaniPredmet.Ocena);
                sqlCmd.Parameters.AddWithValue("BrojBodova", pohadjaniPredmet.BrojBodova);
                sqlCmd.Parameters.AddWithValue("StudentID", pohadjaniPredmet.StudentID);
                try
                {
                    connection.Open();
                    int i = sqlCmd.ExecuteNonQuery();
                    SqlCommand sqlKurs = new SqlCommand("SELECT TOP 1 * FROM Predmet.PohadjaniPredmet ORDER BY PohadjaniPredmetID DESC", connection);
                    PohadjaniPredmet last = new PohadjaniPredmet();
                    using (var dataReader = sqlKurs.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            last.PohadjaniPredmetID = Convert.ToInt32(dataReader["PohadjaniPredmetID"]);
                            last.PredmetID = Convert.ToInt32(dataReader["PredmetID"]);
                            last.Ocena = Convert.ToInt32(dataReader["Ocena"]);
                            last.BrojBodova = Convert.ToInt32(dataReader["BrojBodova"]);
                            last.StudentID = Convert.ToInt32(dataReader["StudentID"]);
                        }
                    }
                    var response = Request.CreateResponse(HttpStatusCode.Created, last);
                    response.Headers.Location = new Uri(Request.RequestUri + "/" + last.PohadjaniPredmetID);
                    return response;
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error 404 Bad request");
                }
            }
        }

        // PUT: api/PohadjaniPredmet/5
        [HttpPut, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public HttpResponseMessage UpdatePohadjaniPredmet([FromBody]PohadjaniPredmet pohadjaniPredmet)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("UPDATE Predmet.PohadjaniPredmet SET PredmetId=@PredmetID,Ocena=@Ocena,BrojBodova=@BrojBodova,StudentID=@StudentID" +
                        "WHERE PohadjaniPredmetID=@PohadjaniPredmetID"))
                    {
                        connection.Open();
                        sqlCmd.Connection = connection;
                        sqlCmd.Parameters.AddWithValue("PohadjaniPredmetID", pohadjaniPredmet.PohadjaniPredmetID);
                        sqlCmd.Parameters.AddWithValue("PredmetID", pohadjaniPredmet.PredmetID);
                        sqlCmd.Parameters.AddWithValue("Ocena", pohadjaniPredmet.Ocena);
                        sqlCmd.Parameters.AddWithValue("BrojBodova", pohadjaniPredmet.BrojBodova);
                        sqlCmd.Parameters.AddWithValue("StudentID", pohadjaniPredmet.StudentID);
                        int rowAffected = sqlCmd.ExecuteNonQuery();
                        if (rowAffected == 0)
                            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error 400 BadRequest");
                    }
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error 400 BadRequest");
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, "Updated Successfully");
        }

        // DELETE: api/PohadjaniPredmet/5
        [HttpDelete, Route("{id}")]
        public HttpResponseMessage DeletePohadjaniPredmet(int id)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("DELETE FROM Predmet.PohadjaniPredmet where PohadjaniPredmetID=@PohadjaniPredmetID"))
                    {
                        connection.Open();
                        sqlCmd.Connection = connection;
                        sqlCmd.Parameters.AddWithValue("PohadjaniPredmetID", id);
                        sqlCmd.ExecuteNonQuery();
                        int rowAffected = sqlCmd.ExecuteNonQuery();
                        if (rowAffected == 0)
                            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Error 404 Not Found");
                    }
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error 404 BadRequest");
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, "Deleted Successfully");
        }
    }

}
