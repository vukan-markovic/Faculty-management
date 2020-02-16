using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Kurs_URIS.AuthFilters;
using Kurs_URIS.Models;
using URIS_v10.Filters;

namespace Kurs_URIS.Controllers
{
    [RoutePrefix("api-kurs/materijal")]
    public class MaterijalController : ApiController
    {
        public string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

        // GET: api/Materijal
        [Route("")]
        [HttpGet]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        [Authorize(Roles = RolesConst.ROLE_Admin)]
        public HttpResponseMessage GetMaterijali()
        {
            List<Materijal> result = new List<Materijal>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Kurs.Materijal", connection)
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
                            var materijal = new Materijal
                            {
                                MaterijalID = Convert.ToInt32(dataReader["MaterijalID"]),
                                NazivMaterijala = Convert.ToString(dataReader["NazivMaterijala"]), 
                                KursID = Convert.ToInt32(dataReader["KursID"])
                            };

                            result.Add(materijal);
                        }
                    }
                }
                catch (Exception )
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error retrieving data");
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        // GET: api/Materijal/5
        [HttpGet, Route("{id}")]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public HttpResponseMessage GetByID(int id)
        {
            Materijal materijal = null;

            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Kurs.Materijal where MaterijalID=" + id, connection)
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
                            materijal = new Materijal
                            {
                                MaterijalID = Convert.ToInt32(dataReader["MaterijalID"]),
                                NazivMaterijala = Convert.ToString(dataReader["NazivMaterijala"]),
                                KursID = Convert.ToInt32(dataReader["KursID"])
                            };
                        }
                    }

                    if (materijal == null)
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Error 404 Found");
                }
                catch (Exception )
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error retrieving data");
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, materijal);
        }

        // POST: api/Materijal
        [HttpPost, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Predavac)]
        public HttpResponseMessage CreateMaterijal([FromBody]Materijal materijal)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("INSERT INTO Kurs.Materijal VALUES(@NazivMaterijala, @KursID)", connection);
                sqlCmd.Parameters.AddWithValue("NazivMaterijala", materijal.NazivMaterijala);
                sqlCmd.Parameters.AddWithValue("KursID", materijal.KursID);

                try
                {
                    connection.Open();
                    int i = sqlCmd.ExecuteNonQuery();
                    SqlCommand sqlMaterijal = new SqlCommand("SELECT TOP 1 * FROM Kurs.Materijal ORDER BY MaterijalID DESC", connection);
                    Materijal last = new Materijal();

                    using (SqlDataReader materijalRead = sqlMaterijal.ExecuteReader())
                    {
                        while (materijalRead.Read())
                        {
                            last.MaterijalID = Convert.ToInt32(materijalRead["MaterijalID"]);
                            last.NazivMaterijala = Convert.ToString(materijalRead["NazivMaterijala"]);
                            last.KursID = Convert.ToInt32(materijalRead["KursID"]);
                        }
                    }

                    var response = Request.CreateResponse(HttpStatusCode.Created, last);
                    response.Headers.Location = new Uri(Request.RequestUri + "/" + last.MaterijalID);
                    return response;
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error 404 Bad request");
                }
            }
        }

        // PUT: api/Materijal/5
        [HttpPut, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Predavac)]
        public HttpResponseMessage UpdateMaterijal([FromBody]Materijal materijal)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("UPDATE Kurs.Materijal SET NazivMaterijala=@NazivMaterijala, KursID=@KursID WHERE MaterijalID=@id"))
                    {
                        connection.Open();
                        sqlCmd.Connection = connection;
                        sqlCmd.Parameters.AddWithValue("NazivMaterijala", materijal.NazivMaterijala);
                        sqlCmd.Parameters.AddWithValue("KursID", materijal.KursID);
                        sqlCmd.Parameters.AddWithValue("id", materijal.MaterijalID);
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

        // DELETE: api/Materijal/5
        [HttpDelete, Route("{id}")]
        [Authorize(Roles = RolesConst.ROLE_Admin_Predavac)]
        public HttpResponseMessage DeleteMaterijal(int id)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("DELETE FROM Kurs.Materijal where MaterijalID=@id"))
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
