using Kurs_URIS.AuthFilters;
using Kurs_URIS.Models;
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
using URIS_v10.Filters;

namespace Kurs_URIS.Controllers
{
    [RoutePrefix("api-kurs/predavac-kurs")]
    public class PredavacKursController : ApiController
    {
        public string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

        // GET: api/PredavacKurs
        [Route("")]
        [HttpGet]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public HttpResponseMessage GetPredavacKurs()
        {
            List<PredavacKurs> result = new List<PredavacKurs>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Kurs.PredavacKurs", connection)
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
                            var predavacKurs = new PredavacKurs
                            {
                                PredavacKursID = Convert.ToInt32(dataReader["PredavacKursID"]),                               
                                KursID = Convert.ToInt32(dataReader["KursID"]),
                                PredavacID = Convert.ToInt32(dataReader["PredavacID"])
                            };

                            result.Add(predavacKurs);
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

        // GET: api/PredavacKurs/5
        [HttpGet, Route("{id}")]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public async Task<HttpResponseMessage> GetByIDAsync(int id)
        {
            PredavacKursWithValueObject predavacOnKurs = new PredavacKursWithValueObject();

            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Kurs.PredavacKurs where KursID=" + id, connection)
                {
                    CommandType = CommandType.Text
                };
                try
                {
                    connection.Open();
                    using (SqlDataReader dataReader = sqlCmd.ExecuteReader())
                    {
                        predavacOnKurs.KursID = id;
                        predavacOnKurs.Predavac = new List<PredavacKursaVO>();
                        while (dataReader.Read())
                        {
                            var predavacID = Convert.ToInt32(dataReader["PredavacID"]);
                            using (var client = new HttpClient())
                            {
                                client.BaseAddress = new Uri("http://localhost:63295/");
                                client.DefaultRequestHeaders.Accept.Clear();
                                var authorization = Request.Headers.GetValues("Authorization").FirstOrDefault();
                                var token = authorization.Split(null)[1];
                                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                HttpResponseMessage response = await client.GetAsync("api-predavac/predavac/" + predavacID);
                                if (response.IsSuccessStatusCode)
                                {
                                    var predavac = await response.Content.ReadAsAsync<PredavacKursaVO>();
                                    predavacOnKurs.Predavac.Add(predavac);
                                }
                            }
                        }
                    }
                    if (predavacOnKurs == null)
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Error 404 Found");
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error retrieving data",ex);
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, predavacOnKurs);
        }

        // POST: api/PredavacKurs
        [HttpPost, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin)]
        public HttpResponseMessage CreatePredavacKurs([FromBody]PredavacKurs predavacKurs)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("INSERT INTO Kurs.PredavacKurs VALUES(@KursID, @PredavacID)", connection);
                sqlCmd.Parameters.AddWithValue("KursID", predavacKurs.KursID);
                sqlCmd.Parameters.AddWithValue("PredavacID", predavacKurs.PredavacID);

                try
                {
                    connection.Open();
                    int i = sqlCmd.ExecuteNonQuery();
                    SqlCommand sqlPredavacKurs = new SqlCommand("SELECT TOP 1 * FROM Kurs.PredavacKurs ORDER BY PredavacKursID DESC", connection);
                    PredavacKurs last = new PredavacKurs();

                    using (SqlDataReader predavacKursRead = sqlPredavacKurs.ExecuteReader())
                    {
                        while (predavacKursRead.Read())
                        {
                            last.PredavacKursID = Convert.ToInt32(predavacKursRead["PredavacKursID"]);
                            last.KursID = Convert.ToInt32(predavacKursRead["KursID"]);
                            last.PredavacID = Convert.ToInt32(predavacKursRead["PredavacID"]);
                        }
                    }

                    var response = Request.CreateResponse(HttpStatusCode.Created, last);
                    response.Headers.Location = new Uri(Request.RequestUri + "/" + last.PredavacKursID);
                    return response;
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error 404 Bad request");
                }
            }
        }

        // PUT: api/PredavacKurs/5
        [HttpPut, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Predavac)]
        public HttpResponseMessage UpdatePredavacKurs([FromBody]PredavacKurs predavacKurs)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("UPDATE Kurs.PredavacKurs SET PredavacID=@PredavacID, KursID=@KursID WHERE PredavacKursID=@id"))
                    {
                        connection.Open();
                        sqlCmd.Connection = connection;
                        sqlCmd.Parameters.AddWithValue("PredavacID", predavacKurs.PredavacID);
                        sqlCmd.Parameters.AddWithValue("KursID", predavacKurs.KursID);
                        sqlCmd.Parameters.AddWithValue("id", predavacKurs.PredavacKursID);

                        int rowAffected = sqlCmd.ExecuteNonQuery();
                        if (rowAffected == 0)
                            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error 400 BadRequest");
                    }
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error 400 Bad Request");
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, "Updated Successfully");
        }

        // DELETE: api/PredavacKurs/5
        [HttpDelete, Route("{id}")]
        [Authorize(Roles = RolesConst.ROLE_Admin)]
        public HttpResponseMessage DeletePredavacKurs(int id)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("DELETE FROM Kurs.PredavacKurs where PredavacKursID=@id"))
                    {
                        connection.Open();
                        sqlCmd.Connection = connection;
                        sqlCmd.Parameters.AddWithValue("id", id);
                        int rowAffected = sqlCmd.ExecuteNonQuery();
                        if (rowAffected == 0)
                            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error 400 BadRequest");
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
