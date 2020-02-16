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
using Tim_URIS.AuthFilters;
using Tim_URIS.Models;
using URIS_v10.Filters;

namespace Tim_URIS.Controllers
{
    [RoutePrefix("api-tim/tim")]
    public class TimController : ApiController
    {
        public string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

        // GET: api/Tim
        [Route("")]
        [HttpGet]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        [Authorize(Roles = RolesConst.ROLE_Admin)]
        public HttpResponseMessage GetTimovi()
        {
            List<Tim> result = new List<Tim>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Tim.Tim", connection)
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
                            var tim = new Tim
                            {
                                TimID = Convert.ToInt32(dataReader["TimID"]),
                                NazivTima = Convert.ToString(dataReader["NazivTima"]),
                                Ocena = Convert.ToInt32(dataReader["Ocena"]),
                                PredavacID = Convert.ToInt32(dataReader["PredavacID"])
                            };

                            result.Add(tim);
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

        // GET: api/Tim/5
        [HttpGet, Route("{id}")]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public async Task<HttpResponseMessage> GetByIDAsync(int id)
        {
            TimWithValueObject tim = null;
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Tim.Tim where TimID=" + id, connection)
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
                            tim = new TimWithValueObject
                            {
                                TimID = Convert.ToInt32(dataReader["TimID"]),
                                NazivTima = Convert.ToString(dataReader["NazivTima"]),
                                Ocena = Convert.ToInt32(dataReader["Ocena"]),
                                PredavacID = Convert.ToInt32(dataReader["PredavacID"])
                            };
                        }
                    }

                    if (tim == null)
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
                var authorization = Request.Headers.GetValues("Authorization").FirstOrDefault();
                var token = authorization.Split(null)[1];
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("api-predavac/predavac/" + tim.PredavacID);

                if (response.IsSuccessStatusCode)
                {
                    var predavac = await response.Content.ReadAsAsync<PredavacInfoVO>();
                    tim.Predavac = predavac;
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, tim);
        }

        // POST: api/Tim
        [HttpPost, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Predavac)]
        public HttpResponseMessage CreateTim([FromBody]Tim tim)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("INSERT INTO Tim.Tim VALUES(@NazivTima, @PredavacID, @Ocena)", connection);
                sqlCmd.Parameters.AddWithValue("NazivTima", tim.NazivTima);
                sqlCmd.Parameters.AddWithValue("PredavacID", tim.PredavacID);
                sqlCmd.Parameters.AddWithValue("Ocena", tim.Ocena);

                try
                {
                    connection.Open();
                    int i = sqlCmd.ExecuteNonQuery();
                    SqlCommand sqlTim = new SqlCommand("SELECT TOP 1 * FROM Tim.Tim ORDER BY TimID DESC", connection);
                    Tim last = new Tim();
                    using (SqlDataReader timRead = sqlTim.ExecuteReader())
                    {
                        while (timRead.Read())
                        {
                            last.TimID = Convert.ToInt32(timRead["TimID"]);
                            last.NazivTima = Convert.ToString(timRead["NazivTima"]);
                            last.Ocena = Convert.ToInt32(timRead["Ocena"]);
                            last.PredavacID = Convert.ToInt32(timRead["PredavacID"]);
                        }
                    }

                    var response = Request.CreateResponse(HttpStatusCode.Created, last);
                    response.Headers.Location = new Uri(Request.RequestUri + "/" + last.TimID);
                    return response;
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error 404 Bad request");
                }
            }
        }

        // PUT: api/Tim/5
        [HttpPut, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Predavac)]
        public HttpResponseMessage UpdateTim([FromBody]Tim tim)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("UPDATE Tim.Tim SET NazivTima=@NazivTima, Ocena=@Ocena, PredavacID=@PredavacID WHERE TimID=@id"))
                    {
                        connection.Open();
                        sqlCmd.Connection = connection;
                        sqlCmd.Parameters.AddWithValue("NazivTima", tim.NazivTima);
                        sqlCmd.Parameters.AddWithValue("Ocena", tim.Ocena);
                        sqlCmd.Parameters.AddWithValue("PredavacID", tim.PredavacID);
                        sqlCmd.Parameters.AddWithValue("id", tim.TimID);
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

        // DELETE: api/Tim/5
        [HttpDelete, Route("{id}")]
        [Authorize(Roles = RolesConst.ROLE_Admin_Predavac)]
        public HttpResponseMessage DeleteTim(int id)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("DELETE FROM Tim.Tim where TimID=@id"))
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
