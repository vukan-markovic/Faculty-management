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
    [RoutePrefix("api-poruka/notifikacija")]
    public class NotifikacijaController : ApiController
    {
        public string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

        // GET: api/Notifikacija
        [Route("")]
        [HttpGet]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        [Authorize(Roles = RolesConst.ROLE_Admin)]
        public HttpResponseMessage GetNotifikacije()
        {
            List<Notifikacija> result = new List<Notifikacija>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Poruka.Notifikacija", connection)
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
                            var notifikacija = new Notifikacija
                            {
                                NotifikacijaID = Convert.ToInt32(dataReader["NotifikacijaID"]),
                                SadrzajNotifikacije = Convert.ToString(dataReader["SadrzajNotifikacije"]), 
                                Ucestalost = Convert.ToString(dataReader["Ucestalost"])
                            };

                            result.Add(notifikacija);
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

        // GET: api/Notifikacija/5
        [HttpGet, Route("{id}")]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public HttpResponseMessage GetByID(int id)
        {
            Notifikacija notifikacija = null;

            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Poruka.Notifikacija where NotifikacijaID=" + id, connection)
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
                            notifikacija = new Notifikacija
                            {
                                NotifikacijaID = Convert.ToInt32(dataReader["NotifikacijaID"]),
                                SadrzajNotifikacije = Convert.ToString(dataReader["SadrzajNotifikacije"]),
                                Ucestalost = Convert.ToString(dataReader["Ucestalost"])
                            };
                        }
                    }

                    if (notifikacija == null)
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Error 404 Found");
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error retrieving data");
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, notifikacija);
        }

        // POST: api/Notifikacija
        [HttpPost, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Predavac)]
        public HttpResponseMessage CreateNotifikacija([FromBody]Notifikacija notifikacija)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("INSERT INTO Poruka.Notifikacija VALUES(@SadrzajNotifikacije, @Ucestalost)", connection);
                sqlCmd.Parameters.AddWithValue("SadrzajNotifikacije", notifikacija.SadrzajNotifikacije);
                sqlCmd.Parameters.AddWithValue("Ucestalost", notifikacija.Ucestalost);

                try
                {
                    connection.Open();
                    int i = sqlCmd.ExecuteNonQuery();
                    SqlCommand sqlNotifikacija = new SqlCommand("SELECT TOP 1 * FROM Poruka.Notifikacija ORDER BY NotifikacijaID DESC", connection);
                    Notifikacija last = new Notifikacija();

                    using (SqlDataReader notifikacijaRead = sqlNotifikacija.ExecuteReader())
                    {
                        while (notifikacijaRead.Read())
                        {
                            last.NotifikacijaID = Convert.ToInt32(notifikacijaRead["NotifikacijaID"]);
                            last.SadrzajNotifikacije = Convert.ToString(notifikacijaRead["SadrzajNotifikacije"]);
                            last.Ucestalost = Convert.ToString(notifikacijaRead["Ucestalost"]);
                        }
                    }

                    var response = Request.CreateResponse(HttpStatusCode.Created, last);
                    response.Headers.Location = new Uri(Request.RequestUri + "/" + last.NotifikacijaID);
                    return response;
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error 404 Bad request");
                }
            }
        }

        // PUT: api/Notifikacija/5
        [HttpPut, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Predavac)]
        public HttpResponseMessage UpdateNotifikacija([FromBody]Notifikacija notifikacija)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("UPDATE Poruka.Notifikacija SET SadrzajNotifikacije=@SadrzajNotifikacije, Ucestalost=@Ucestalost WHERE NotifikacijaID=@id"))
                    {
                        connection.Open();
                        sqlCmd.Connection = connection;
                        sqlCmd.Parameters.AddWithValue("SadrzajNotifikacije", notifikacija.SadrzajNotifikacije);
                        sqlCmd.Parameters.AddWithValue("Ucestalost", notifikacija.Ucestalost);
                        sqlCmd.Parameters.AddWithValue("id", notifikacija.NotifikacijaID);
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

        // DELETE: api/Notifikacija/5
        [HttpDelete, Route("{id}")]
        [Authorize(Roles = RolesConst.ROLE_Admin_Predavac)]
        public HttpResponseMessage DeleteNotifikacija(int id)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("DELETE FROM Poruka.Notifikacija where NotifikacijaID=@id"))
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
