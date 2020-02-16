using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Poruka_URIS.AuthFilters;
using Poruka_URIS.Models;
using URIS_v10.Filters;

namespace Poruka_URIS.Controllers
{
    [RoutePrefix("api-poruka/dogadjaj")]
    public class DogadjajController : ApiController
    {
        public string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

        // GET: api/Dogadjaj
        [Route("")]
        [HttpGet]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        [Authorize(Roles = RolesConst.ROLE_Admin)]
        public HttpResponseMessage GetDogadjaji()
        {
            List<Dogadjaj> result = new List<Dogadjaj>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Poruka.Dogadjaj", connection)
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
                            var dogadjaj = new Dogadjaj
                            {
                                DogadjajID = Convert.ToInt32(dataReader["DogadjajID"]),
                                NazivDogadjaja = Convert.ToString(dataReader["NazivDogadjaja"]), 
                                NotifikacijaID = Convert.ToInt32(dataReader["NotifikacijaID"])
                            };

                            result.Add(dogadjaj);
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

        // GET: api/Dogadjaj/5
        [HttpGet, Route("{id}")]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public HttpResponseMessage GetByID(int id)
        {
            Dogadjaj dogadjaj = null;

            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Poruka.Dogadjaj where DogadjajID=" + id, connection)
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
                            dogadjaj = new Dogadjaj
                            {
                                DogadjajID = Convert.ToInt32(dataReader["DogadjajID"]),
                                NazivDogadjaja = Convert.ToString(dataReader["NazivDogadjaja"]),
                                NotifikacijaID = Convert.ToInt32(dataReader["NotifikacijaID"])
                            };
                        }
                    }

                    if (dogadjaj == null)
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Error 404 Found");
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error retrieving data");
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, dogadjaj);
        }

        // POST: api/Dogadjaj
        [HttpPost, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Predavac)]
        public HttpResponseMessage CreateDogadjaj([FromBody]Dogadjaj dogadjaj)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("INSERT INTO Poruka.Dogadjaj VALUES(@NazivDogadjaja, @NotifikacijaID)", connection);
                sqlCmd.Parameters.AddWithValue("NazivDogadjaja", dogadjaj.NazivDogadjaja);
                sqlCmd.Parameters.AddWithValue("NotifikacijaID", dogadjaj.NotifikacijaID);

                try
                {
                    connection.Open();
                    int i = sqlCmd.ExecuteNonQuery();
                    SqlCommand sqlUniverzitet = new SqlCommand("SELECT TOP 1 * FROM Poruka.Dogadjaj ORDER BY DogadjajID DESC", connection);
                    Dogadjaj last = new Dogadjaj();

                    using (SqlDataReader dogadjajRead = sqlUniverzitet.ExecuteReader())
                    {
                        while (dogadjajRead.Read())
                        {
                            last.DogadjajID = Convert.ToInt32(dogadjajRead["DogadjajID"]);
                            last.NazivDogadjaja = Convert.ToString(dogadjajRead["NazivDogadjaja"]);
                            last.NotifikacijaID = Convert.ToInt32(dogadjajRead["NotifikacijaID"]);
                        }
                    }

                    var response = Request.CreateResponse(HttpStatusCode.Created, last);
                    response.Headers.Location = new Uri(Request.RequestUri + "/" + last.DogadjajID);
                    return response;
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error 404 Bad request");
                }
            }
        }

        // PUT: api/Dogadjaj/5
        [HttpPut, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Predavac)]
        public HttpResponseMessage UpdateDogadjaj([FromBody] Dogadjaj dogadjaj)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("UPDATE Poruka.Dogadjaj SET NazivDogadjaja=@NazivDogadjaja, NotifikacijaID=@NotifikacijaID WHERE DogadjajID=@id"))
                    {
                        connection.Open();
                        sqlCmd.Connection = connection;
                        sqlCmd.Parameters.AddWithValue("NazivDogadjaja", dogadjaj.NazivDogadjaja);
                        sqlCmd.Parameters.AddWithValue("NotifikacijaID", dogadjaj.NotifikacijaID);
                        sqlCmd.Parameters.AddWithValue("id", dogadjaj.DogadjajID);
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

        // DELETE: api/Dogadjaj/5
        [HttpDelete, Route("{id}")]
        [Authorize(Roles = RolesConst.ROLE_Admin_Predavac)]
        public HttpResponseMessage DeleteDogadjaj(int id)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("DELETE FROM Poruka.Dogadjaj where DogadjajID=@id"))
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
