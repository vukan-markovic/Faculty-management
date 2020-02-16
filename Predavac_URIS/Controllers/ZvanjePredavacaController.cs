using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Predavac_URIS.AuthFilters;
using Predavac_URIS.Models;
using URIS_v10.Filters;

namespace Predavac_URIS.Controllers
{
    [RoutePrefix("api-predavac/zvanje_predavaca")]
    public class ZvanjePredavacaController: ApiController
    {
        public string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

        // GET: api/ZvanjePredavaca
        [Route("")]
        [HttpGet]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public HttpResponseMessage GetZvanjaPredavaca()
        {
            List<ZvanjePredavaca> result = new List<ZvanjePredavaca>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Predavac.ZvanjePredavaca", connection)
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
                            var zvanjePredavaca = new ZvanjePredavaca
                            {
                                ZvanjePredavacaID = Convert.ToInt32(dataReader["ZvanjePredavacaID"]),
                                NazivZvanjaPredavaca = Convert.ToString(dataReader["NazivZvanjaPredavaca"])
                            };

                            result.Add(zvanjePredavaca);
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

        // GET: api/ZvanjePredavaca/5
        [HttpGet, Route("{id}")]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public HttpResponseMessage GetByID(int id)
        {
            ZvanjePredavaca zvanjePredavaca = null;
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Predavac.ZvanjePredavaca where ZvanjePredavacaID=" + id, connection)
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
                            zvanjePredavaca = new ZvanjePredavaca
                            {
                                ZvanjePredavacaID = Convert.ToInt32(dataReader["ZvanjePredavacaID"]),
                                NazivZvanjaPredavaca = Convert.ToString(dataReader["NazivZvanjaPredavaca"])
                            };
                        }
                    }

                    if (zvanjePredavaca == null)
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Error 404 Found");
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error retrieving data");
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, zvanjePredavaca);
        }

        // POST: api/ZvanjePredavaca
        [HttpPost, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin)]
        public HttpResponseMessage CreateZvanjePredavaca([FromBody]ZvanjePredavaca zvanjePredavaca)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("INSERT INTO Predavac.ZvanjePredavaca VALUES(@NazivZvanjaPredavaca)", connection);
                sqlCmd.Parameters.AddWithValue("NazivZvanjaPredavaca", zvanjePredavaca.NazivZvanjaPredavaca);

                try
                {
                    connection.Open();
                    int i = sqlCmd.ExecuteNonQuery();
                    SqlCommand sqlZvanjePredavaca = new SqlCommand("SELECT TOP 1 * FROM Predavac.ZvanjePredavaca ORDER BY ZvanjePredavacaID DESC", connection);
                    ZvanjePredavaca last = new ZvanjePredavaca();
                    using (SqlDataReader zvanjePredavacaRead = sqlZvanjePredavaca.ExecuteReader())
                    {
                        while (zvanjePredavacaRead.Read())
                        {
                            last.ZvanjePredavacaID = Convert.ToInt32(zvanjePredavacaRead["ZvanjePredavacaID"]);
                            last.NazivZvanjaPredavaca = Convert.ToString(zvanjePredavacaRead["NazivZvanjaPredavaca"]);
                        }
                    }

                    var response = Request.CreateResponse(HttpStatusCode.Created, last);
                    response.Headers.Location = new Uri(Request.RequestUri + "/" + last.ZvanjePredavacaID);
                    return response;
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error 404 Bad request");
                }
            }
        }

        // PUT: api/ZvanjePredavaca/5
        [HttpPut, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Predavac)]
        public HttpResponseMessage UpdateZvanjePredavaca([FromBody]ZvanjePredavaca zvanjePredavaca)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("UPDATE Predavac.ZvanjePredavaca SET NazivZvanjaPredavaca=@NazivZvanjaPredavaca WHERE ZvanjePredavacaID=@id"))
                    {
                        connection.Open();
                        sqlCmd.Connection = connection;
                        sqlCmd.Parameters.AddWithValue("NazivZvanjaPredavaca", zvanjePredavaca.NazivZvanjaPredavaca);
                        sqlCmd.Parameters.AddWithValue("id", zvanjePredavaca.ZvanjePredavacaID);
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

        // DELETE: api/ZvanjePredavaca/5
        [HttpDelete, Route("{id}")]
        [Authorize(Roles = RolesConst.ROLE_Admin_Predavac)]
        public HttpResponseMessage DeleteZvanjePredavaca(int id)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("DELETE FROM Predavac.ZvanjePredavaca where ZvanjePredavacaID=@id"))
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