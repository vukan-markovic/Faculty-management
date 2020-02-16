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
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace Predmet_URIS.Controllers
{
    [RoutePrefix("api-predmet/predmet")]
    public class PredmetController : ApiController
    {
        public string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

        // GET: api/Predmet
        [Route("")]
        [HttpGet]
        [Authorize(Roles=RolesConst.ROLE_Admin_Student_Predavac)]
        [ClientCacheControlFilter(ClientCacheControl.Private,5)]
        public HttpResponseMessage GetPredmeti()
        {
            List<Predmet> result = new List<Predmet>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Predmet.Predmet", connection)
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
                            var predmet = new Predmet
                            {
                                PredmetID = Convert.ToInt32(dataReader["PredmetID"]),
                                NazivPredmeta = Convert.ToString(dataReader["NazivPredmeta"]),
                                OznakaPredmeta=Convert.ToString(dataReader["OznakaPredmeta"]),
                                Godina=Convert.ToInt32(dataReader["Godina"]),
                                ECTSBodovi=Convert.ToInt32(dataReader["ECTSBodovi"]),
                                DepartmanID=Convert.ToInt32(dataReader["DepartmanID"])
                            };
                            result.Add(predmet);
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

        // GET: api/Predmet/5
        [HttpGet, Route("{id}")]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public async Task<HttpResponseMessage> GetByIDAsync(int id)
        {
            PredmetWithValueObject predmet = null;
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Predmet.Predmet where PredmetID=" + id, connection)
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
                            predmet = new PredmetWithValueObject
                            {
                                PredmetID = Convert.ToInt32(dataReader["PredmetID"]),
                                NazivPredmeta = Convert.ToString(dataReader["NazivPredmeta"]),
                                OznakaPredmeta = Convert.ToString(dataReader["OznakaPredmeta"]),
                                Godina = Convert.ToInt32(dataReader["Godina"]),
                                ECTSBodovi = Convert.ToInt32(dataReader["ECTSBodovi"]),
                                DepartmanID = Convert.ToInt32(dataReader["DepartmanID"])
                            };
                        }
                    }
                    if(predmet==null)
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Error 404 Found");
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error retrieving data");
                }
            }
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:63298/");
                client.DefaultRequestHeaders.Accept.Clear();
                var authorization = Request.Headers.GetValues("Authorization").FirstOrDefault();
                var token = authorization.Split(null)[1];
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                using (var response = await client.GetAsync("api-departman/departman/"+predmet.DepartmanID))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var Departman = await response.Content.ReadAsAsync<DepartmanPredmetaVO>();
                        predmet.Departman = Departman;
                    }
                } 
            }
            return Request.CreateResponse(HttpStatusCode.OK, predmet);
        }

        // POST: api/Predmet
        [HttpPost, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin)]
        public HttpResponseMessage CreatePredmet([FromBody]Predmet predmet)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("INSERT INTO Predmet.Predmet VALUES(@NazivPredmeta,@OznakaPredmeta,@ECTSBodovi,@DepartmanID,@Godina)", connection);
                sqlCmd.Parameters.AddWithValue("NazivPredmeta", predmet.NazivPredmeta);
                sqlCmd.Parameters.AddWithValue("OznakaPredmeta", predmet.OznakaPredmeta);
                sqlCmd.Parameters.AddWithValue("Godina", predmet.Godina);
                sqlCmd.Parameters.AddWithValue("ECTSBodovi", predmet.ECTSBodovi);
                sqlCmd.Parameters.AddWithValue("DepartmanID", predmet.DepartmanID);
                try
                {
                    connection.Open();
                    sqlCmd.ExecuteNonQuery();
                    SqlCommand sqlKurs = new SqlCommand("SELECT TOP 1 * FROM Predmet.Predmet ORDER BY PredmetID DESC", connection);
                    Predmet last = new Predmet();
                    using (var dataReader = sqlKurs.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            last.PredmetID = Convert.ToInt32(dataReader["PredmetID"]);
                            last.NazivPredmeta = Convert.ToString(dataReader["NazivPredmeta"]);
                            last.OznakaPredmeta = Convert.ToString(dataReader["OznakaPredmeta"]);
                            last.Godina = Convert.ToInt32(dataReader["Godina"]);
                            last.ECTSBodovi = Convert.ToInt32(dataReader["ECTSBodovi"]);
                            last.DepartmanID = Convert.ToInt32(dataReader["DepartmanID"]);
                        }
                    }
                    var response = Request.CreateResponse(HttpStatusCode.Created, last);
                    response.Headers.Location = new Uri(Request.RequestUri + "/" + last.PredmetID);
                    return response;
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error 404 Bad request");
                }
            }
        }

        // PUT: api/Predmet/5
        [HttpPut, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Predavac)]
        public HttpResponseMessage UpdatePredmet([FromBody]Predmet predmet)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("UPDATE Predmet.Predmet SET NazivPredmeta=@NazivPredmeta," +
                        "OznakaPredmeta=@OznakaPredmeta,Godina=@Godina,ECTSBodovi=@ECTSBodovi,DepartmanID=@DepartmanID WHERE PredmetID=@PredmetID"))
                    {
                        connection.Open();
                        sqlCmd.Connection = connection;
                        sqlCmd.Parameters.AddWithValue("PredmetID", predmet.PredmetID);
                        sqlCmd.Parameters.AddWithValue("NazivPredmeta", predmet.NazivPredmeta);
                        sqlCmd.Parameters.AddWithValue("OznakaPredmeta", predmet.OznakaPredmeta);
                        sqlCmd.Parameters.AddWithValue("Godina", predmet.Godina);
                        sqlCmd.Parameters.AddWithValue("ECTSBodovi", predmet.ECTSBodovi);
                        sqlCmd.Parameters.AddWithValue("DepartmanID", predmet.DepartmanID);
                        int rowAffected=sqlCmd.ExecuteNonQuery();
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

        // DELETE: api/Predmet/5
        [HttpDelete, Route("{id}")]
        [Authorize(Roles = RolesConst.ROLE_Admin)]
        public HttpResponseMessage DeletePredmet(int id)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("DELETE FROM Predmet.Predmet where PredmetID=@PredmetID"))
                    {
                        connection.Open();
                        sqlCmd.Connection = connection;
                        sqlCmd.Parameters.AddWithValue("PredmetID", id);
                        int rowAffected=sqlCmd.ExecuteNonQuery();
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

