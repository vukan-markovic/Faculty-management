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
    [RoutePrefix("api-departman/fakultet")]
    public class FakultetController : ApiController
    {
        public string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

        // GET: api/Univerzitet
        [Route("")]
        [HttpGet]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        public HttpResponseMessage GetFakulteti()
        {
            List<Fakultet> result = new List<Fakultet>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Departman.Fakultet", connection)
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
                            var fakultet = new Fakultet
                            {
                                FakultetID = Convert.ToInt32(dataReader["FakultetID"]),
                                NazivFakulteta = Convert.ToString(dataReader["NazivFakulteta"]),
                                UniverzitetID = Convert.ToInt32(dataReader["UniverzitetID"])
                            };

                            result.Add(fakultet);
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

        // GET: api/Fakultet/5
        [HttpGet, Route("{id}")]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        public HttpResponseMessage GetByID(int id)
        {
            Fakultet fakultet = null;
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Departman.Fakultet where FakultetID=" + id, connection)
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
                            fakultet = new Fakultet
                            {
                                FakultetID = Convert.ToInt32(dataReader["FakultetID"]),
                                NazivFakulteta = Convert.ToString(dataReader["NazivFakulteta"]),
                                UniverzitetID = Convert.ToInt32(dataReader["UniverzitetID"])
                            };
                        }
                    }
                    if (fakultet == null)
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Error 404 Found");
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error retrieving data");
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, fakultet);
        }

        // POST: api/Fakultet
        [HttpPost, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin)]
        public HttpResponseMessage CreateFakultet([FromBody]Fakultet fakultet)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("INSERT INTO Departman.Fakultet VALUES(@NazivFakulteta, @UniverzitetID)", connection);
                sqlCmd.Parameters.AddWithValue("NazivFakulteta", fakultet.NazivFakulteta);
                sqlCmd.Parameters.AddWithValue("UniverzitetID", fakultet.UniverzitetID);
                try
                {
                    connection.Open();
                    int i = sqlCmd.ExecuteNonQuery();
                    SqlCommand sqlFakultet = new SqlCommand("SELECT TOP 1 * FROM Departman.Fakultet ORDER BY FakultetID DESC", connection);
                    Fakultet last = new Fakultet();
                    using (SqlDataReader fakultetRead = sqlFakultet.ExecuteReader())
                    {
                        while (fakultetRead.Read())
                        {
                            last.FakultetID = Convert.ToInt32(fakultetRead["FakultetID"]);
                            last.NazivFakulteta = Convert.ToString(fakultetRead["NazivFakulteta"]);
                            last.UniverzitetID = Convert.ToInt32(fakultetRead["UniverzitetID"]);
                        }
                    }

                    var response = Request.CreateResponse(HttpStatusCode.Created, last);
                    response.Headers.Location = new Uri(Request.RequestUri + "/" + last.FakultetID);
                    return response;
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error 404 Bad request");
                }
            }
        }

        // PUT: api/Fakultet/5
        [HttpPut, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin)]
        public HttpResponseMessage UpdatePredmet([FromBody]Fakultet fakultet)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("UPDATE Departman.Fakultet SET NazivFakulteta=@NazivFakulteta, UniverzitetID=@UniverzitetID WHERE FakultetID=@FakultetID"))
                    {
                        connection.Open();
                        sqlCmd.Connection = connection;
                        sqlCmd.Parameters.AddWithValue("NazivFakulteta", fakultet.NazivFakulteta);
                        sqlCmd.Parameters.AddWithValue("FakultetID", fakultet.FakultetID);
                        sqlCmd.Parameters.AddWithValue("UniverzitetID", fakultet.UniverzitetID);
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

        // DELETE: api/Fakultet/5
        [HttpDelete, Route("{id}")]
        [Authorize(Roles = RolesConst.ROLE_Admin)]
        public HttpResponseMessage DeleteFakultet(int id)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("DELETE FROM Departman.Fakultet where FakultetID=@id"))
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
