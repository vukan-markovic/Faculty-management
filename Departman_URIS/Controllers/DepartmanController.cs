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
using System.Net.Http.Headers;
using System.Web.Http;

namespace DepartmanURIS.Controllers
{
    [RoutePrefix("api-departman/departman")]
    public class DepartmanController : ApiController
    {
        public string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

        // GET: api/Departman
        [Route("")]
        [HttpGet]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        public HttpResponseMessage GetDepartmani()
        {
            List<Departman> result = new List<Departman>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Departman.Departman", connection)
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
                            var Departman = new Departman
                            {
                                DepartmanID = Convert.ToInt32(dataReader["DepartmanID"]),
                                NazivDepartmana = Convert.ToString(dataReader["NazivDepartmana"]),
                                FakultetID = Convert.ToInt32(dataReader["FakultetID"])
                            };

                            result.Add(Departman);
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

        // GET: api/Departman/5
        [HttpGet, Route("{id}")]
        [Authorize(Roles =RolesConst.ROLE_Admin_Student_Predavac)]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        public async System.Threading.Tasks.Task<HttpResponseMessage> GetDepartmanByIDAsync(int id)
        {
            DepartmanWithVO departman = null;
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Departman.Departman where DepartmanID=" + id, connection)
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
                            departman = new DepartmanWithVO
                            {
                                DepartmanID = Convert.ToInt32(dataReader["DepartmanID"]),
                                NazivDepartmana = Convert.ToString(dataReader["NazivDepartmana"]),
                                FakultetID = Convert.ToInt32(dataReader["FakultetID"])
                            };
                        }
                    }
                    if (departman == null)
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Error 404 Found");

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://localhost:63295/");
                        client.DefaultRequestHeaders.Accept.Clear();
                        var authorization = Request.Headers.GetValues("Authorization").FirstOrDefault();
                        var token = authorization.Split(null)[1];
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        using (var response = await client.GetAsync("api-predavac/predavac/departman/" + departman.DepartmanID))
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                var predavac = await response.Content.ReadAsAsync<List<PredavacVO>>();
                                departman.Predavac = predavac;
                            }
                        }
                    }
                }
                
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error retrieving data",ex);
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, departman);
        }

        // POST: api/Departman
        [HttpPost, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin)]
        public HttpResponseMessage CreateDepartman([FromBody]Departman Departman)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("INSERT INTO Departman.Departman VALUES(@NazivDepartmana, @FakultetID)", connection);
                sqlCmd.Parameters.AddWithValue("NazivDepartmana", Departman.NazivDepartmana);
                sqlCmd.Parameters.AddWithValue("FakultetID", Departman.FakultetID);
                try
                {
                    connection.Open();
                    int i = sqlCmd.ExecuteNonQuery();
                    SqlCommand sqlDepartman = new SqlCommand("SELECT TOP 1 * FROM Departman.Departman ORDER BY DepartmanID DESC", connection);
                    Departman last = new Departman();
                    using (var DepartmanRead = sqlDepartman.ExecuteReader())
                    {
                        while (DepartmanRead.Read())
                        {
                            last.DepartmanID = Convert.ToInt32(DepartmanRead["DepartmanID"]);
                            last.NazivDepartmana = Convert.ToString(DepartmanRead["NazivDepartmana"]);
                            last.FakultetID = Convert.ToInt32(DepartmanRead["FakultetID"]);
                        }
                    }

                    var response = Request.CreateResponse(HttpStatusCode.Created, last);
                    response.Headers.Location = new Uri(Request.RequestUri + "/" + last.DepartmanID);
                    return response;
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error 404 Bad request");
                }
            }
        }

        // PUT: api/Departman/5
        [HttpPut, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin)]
        public HttpResponseMessage UpdatePredmet([FromBody]Departman Departman)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("UPDATE Departman.Departman SET NazivDepartmana=@NazivDepartmana, FakultetID=@FakultetID WHERE Departman.DepartmanID=@DepartmanID"))
                    {
                        connection.Open();
                        sqlCmd.Connection = connection;
                        sqlCmd.Parameters.AddWithValue("NazivDepartmana", Departman.NazivDepartmana);
                        sqlCmd.Parameters.AddWithValue("DepartmanID", Departman.DepartmanID);
                        sqlCmd.Parameters.AddWithValue("FakultetID", Departman.FakultetID);
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

        // DELETE: api/Departman/5
        [HttpDelete, Route("{id}")]
        [Authorize(Roles = RolesConst.ROLE_Admin)]
        public HttpResponseMessage DeleteDepartman(int id)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("DELETE FROM Departman.Departman where Departman.DepartmanID=@id"))
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
