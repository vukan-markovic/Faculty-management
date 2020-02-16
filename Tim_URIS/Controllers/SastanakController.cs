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
    [RoutePrefix("api-tim/sastanak")]
    public class SastanakController : ApiController
    {
        public string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

        // GET: api/Sastanak
        [Route("")]
        [HttpGet]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        [Authorize(Roles = RolesConst.ROLE_Admin)]
        public HttpResponseMessage GetSastanci()
        {
            List<Sastanak> result = new List<Sastanak>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Tim.Sastanak", connection)
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
                            var sastanak = new Sastanak
                            {
                                SastanakID = Convert.ToInt32(dataReader["SastanakID"]),
                                PovodSastanka = Convert.ToString(dataReader["PovodSastanka"]),
                                MestoSastanka = Convert.ToString(dataReader["MestoSastanka"]),
                                PredavacID = Convert.ToInt32(dataReader["PredavacID"]),
                                TimID = Convert.ToInt32(dataReader["TimID"]),
                                VremeSastanka = Convert.ToDateTime(dataReader["VremeSastanka"])
                            };

                            result.Add(sastanak);
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

        // GET: api/Sastanak/5
        [HttpGet, Route("{id}")]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public async Task<HttpResponseMessage> GetByIDAsync(int id)
        {
            SastanakWithValueObject sastanak = null;
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Tim.Sastanak where SastanakID=" + id, connection)
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
                            sastanak = new SastanakWithValueObject
                            {
                                SastanakID = Convert.ToInt32(dataReader["SastanakID"]),
                                PovodSastanka = Convert.ToString(dataReader["PovodSastanka"]),
                                MestoSastanka = Convert.ToString(dataReader["MestoSastanka"]),
                                PredavacID = Convert.ToInt32(dataReader["PredavacID"]),
                                TimID = Convert.ToInt32(dataReader["TimID"]),
                                VremeSastanka = Convert.ToDateTime(dataReader["VremeSastanka"])
                            };
                        }
                    }

                    if (sastanak == null)
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Error 404 Found");
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error retrieving data");
                }
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:63295/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("api-predavac/predavac/" + sastanak.PredavacID);

                if (response.IsSuccessStatusCode)
                {
                    var predavac = await response.Content.ReadAsAsync<PredavacInfoVO>();
                    sastanak.Predavac = predavac;
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, sastanak);
        }

        // POST: api/Sastanak
        [HttpPost, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Predavac)]
        public HttpResponseMessage CreateSastanak([FromBody]Sastanak sastanak)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("INSERT INTO Tim.Sastanak VALUES(@TimID, @PredavacID, @VremeSastanka, @MestoSastanka, @PovodSastanka)", connection);
                sqlCmd.Parameters.AddWithValue("TimID", sastanak.TimID);
                sqlCmd.Parameters.AddWithValue("PredavacID", sastanak.PredavacID);
                sqlCmd.Parameters.AddWithValue("VremeSastanka", sastanak.VremeSastanka);
                sqlCmd.Parameters.AddWithValue("MestoSastanka", sastanak.MestoSastanka);
                sqlCmd.Parameters.AddWithValue("PovodSastanka", sastanak.PovodSastanka);

                try
                {
                    connection.Open();
                    int i = sqlCmd.ExecuteNonQuery();
                    SqlCommand sqlSastanak = new SqlCommand("SELECT TOP 1 * FROM Tim.Sastanak ORDER BY SastanakID DESC", connection);
                    Sastanak last = new Sastanak();
                    using (SqlDataReader sastanakRead = sqlSastanak.ExecuteReader())
                    {
                        while (sastanakRead.Read())
                        {
                            last.SastanakID = Convert.ToInt32(sastanakRead["SastanakID"]);
                            last.PovodSastanka = Convert.ToString(sastanakRead["PovodSastanka"]);
                            last.MestoSastanka = Convert.ToString(sastanakRead["MestoSastanka"]);
                            last.PredavacID = Convert.ToInt32(sastanakRead["PredavacID"]);
                            last.TimID = Convert.ToInt32(sastanakRead["TimID"]);
                            last.VremeSastanka = Convert.ToDateTime(sastanakRead["VremeSastanka"]);
                        }
                    }

                    var response = Request.CreateResponse(HttpStatusCode.Created, last);
                    response.Headers.Location = new Uri(Request.RequestUri + "/" + last.SastanakID);
                    return response;
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error 404 Bad request");
                }
            }
        }

        // PUT: api/Sastanak/5
        [HttpPut, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public HttpResponseMessage UpdateSastanak([FromBody]Sastanak sastanak)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("UPDATE Tim.Sastanak SET PovodSastanka=@PovodSastanka, MestoSastanka=@MestoSastanka, PredavacID=@PredavacID, TimID=@TimID, VremeSastanka=@VremeSastanka WHERE SastanakID=@id"))
                    {
                        connection.Open();
                        sqlCmd.Connection = connection;
                        sqlCmd.Parameters.AddWithValue("PovodSastanka", sastanak.PovodSastanka);
                        sqlCmd.Parameters.AddWithValue("MestoSastanka", sastanak.MestoSastanka);
                        sqlCmd.Parameters.AddWithValue("PredavacID", sastanak.PredavacID);
                        sqlCmd.Parameters.AddWithValue("TimID", sastanak.TimID);
                        sqlCmd.Parameters.AddWithValue("VremeSastanka", sastanak.VremeSastanka);
                        sqlCmd.Parameters.AddWithValue("id", sastanak.SastanakID);
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

        // DELETE: api/Sastanak/5
        [HttpDelete, Route("{id}")]
        [Authorize(Roles = RolesConst.ROLE_Admin_Predavac)]
        public HttpResponseMessage DeleteSastanak(int id)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("DELETE FROM Tim.Sastanak where SastanakID=@id"))
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
