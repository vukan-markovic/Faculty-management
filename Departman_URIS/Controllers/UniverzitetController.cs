using Departman_URIS.Filter;
using Departman_URIS.Filters;
using Departman_URIS.Models;
using DepartmanURIS.AuthFilters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Departman_URIS.Controllers
{
    [RoutePrefix("api-departman/univerzitet")]
    public class UniverzitetController : ApiController
    {
        public string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

        // GET: api/Univerzitet
        [Route("")]
        [HttpGet]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        public HttpResponseMessage GetUniverziteti()
        {
            List<Univerzitet> result = new List<Univerzitet>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Departman.Univerzitet", connection)
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
                            var univerzitet = new Univerzitet
                            {
                                UniverzitetID = Convert.ToInt32(dataReader["UniverzitetID"]),
                                NazivUniverziteta = Convert.ToString(dataReader["NazivUniverziteta"])
                            };
                            result.Add(univerzitet);
                        }
                    }
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error retrieving data",ex);
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        // GET: api/Univerzitet/5
        [HttpGet, Route("{id}")]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        public HttpResponseMessage GetUniverzitetByID(int id)
        {
            Univerzitet univerzitet = null;
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Departman.Univerzitet where UniverzitetID=" + id, connection)
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
                            univerzitet = new Univerzitet
                            {
                                UniverzitetID = Convert.ToInt32(dataReader["UniverzitetID"]),
                                NazivUniverziteta = Convert.ToString(dataReader["NazivUniverziteta"])
                            };
                        }
                    }
                    if (univerzitet == null)
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Error 404 Found");
                }
                catch (Exception )
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error retrieving data");
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, univerzitet);
        }

        // POST: api/Univerzitet
        [HttpPost, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin)]
        public HttpResponseMessage CreateUniverzitet([FromBody]Univerzitet univerzitet)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("INSERT INTO Departman.Univerzitet VALUES(@NazivUniverziteta)", connection);
                sqlCmd.Parameters.AddWithValue("NazivUniverziteta", univerzitet.NazivUniverziteta);
                try
                {
                    connection.Open();
                    int i = sqlCmd.ExecuteNonQuery();
                    SqlCommand sqlUniverzitet = new SqlCommand("SELECT TOP 1 * FROM Departman.Univerzitet ORDER BY UniverzitetID DESC", connection);
                    Univerzitet last = new Univerzitet();
                    using (var univerzitetRead = sqlUniverzitet.ExecuteReader())
                    {
                        while (univerzitetRead.Read())
                        {
                            last.UniverzitetID = Convert.ToInt32(univerzitetRead["UniverzitetID"]);
                            last.NazivUniverziteta = Convert.ToString(univerzitetRead["NazivUniverziteta"]);
                        }
                    }

                    var response = Request.CreateResponse(HttpStatusCode.Created, last);
                    response.Headers.Location = new Uri(Request.RequestUri + "/" + last.UniverzitetID);
                    return response;
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error 404 Bad request",ex);
                }
            }
        }

        // PUT: api/Predmet/5
        [HttpPut, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin)]
        public HttpResponseMessage UpdateUniverzitet([FromBody]Univerzitet univerzitet)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("UPDATE Departman.Univerzitet SET NazivUniverziteta=@NazivUniverziteta WHERE UniverzitetID=@id"))
                    {
                        connection.Open();
                        sqlCmd.Connection = connection;
                        sqlCmd.Parameters.AddWithValue("NazivUniverziteta", univerzitet.NazivUniverziteta);
                        sqlCmd.Parameters.AddWithValue("id", univerzitet.UniverzitetID);
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

        // DELETE: api/Univerzitet/5
        [HttpDelete, Route("{id}")]
        [Authorize(Roles = RolesConst.ROLE_Admin)]
        public HttpResponseMessage DeleteUniverzitet(int id)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("DELETE FROM Departman.Univerzitet where UniverzitetID=@id"))
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
