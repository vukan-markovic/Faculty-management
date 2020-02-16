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
using Poruka_URIS.AuthFilters;
using Poruka_URIS.Models;
using URIS_v10.Filters;

namespace Poruka_URIS.Controllers
{
    [RoutePrefix("api-poruka/poruka")]
    public class PorukaController : ApiController
    {
        public string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

        // GET: api/Poruka
        [Route("")]
        [HttpGet]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        [Authorize(Roles = RolesConst.ROLE_Admin)]
        public HttpResponseMessage GetPoruke()
        {
            List<Poruka> result = new List<Poruka>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Poruka.Poruka", connection)
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
                            var poruka = new Poruka
                            {
                                PorukaID = Convert.ToInt32(dataReader["PorukaID"])
                            };

                            if (!DBNull.Value.Equals(dataReader["PredavacID"]))
                            poruka.PredavacID = Convert.ToInt32(dataReader["PredavacID"]);

                            poruka.SadrzajPoruke = Convert.ToString(dataReader["SadrzajPoruke"]);

                            if (!DBNull.Value.Equals(dataReader["StudentID"]))
                            poruka.StudentID = Convert.ToInt32(dataReader["StudentID"]);

                            if (!DBNull.Value.Equals(dataReader["TimID"]))
                            poruka.TimID = Convert.ToInt32(dataReader["TimID"]);

                            poruka.VidljivostPoruke = Convert.ToString(dataReader["VidljivostPoruke"]);
                            poruka.VrstaPoruke = Convert.ToString(dataReader["VrstaPoruke"]);
                            poruka.NotifikacijaID = Convert.ToInt32(dataReader["NotifikacijaID"]);
               
                            result.Add(poruka);
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

        // GET: api/Poruka/5
        [HttpGet, Route("{id}")]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public async Task<HttpResponseMessage> GetByIDAsync(int id)
        {
            PorukaWithValueObject poruka = null;
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Poruka.Poruka where PorukaID=" + id, connection)
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
                            poruka = new PorukaWithValueObject
                            {
                                PorukaID = Convert.ToInt32(dataReader["PorukaID"])
                            };

                            if (!DBNull.Value.Equals(dataReader["PredavacID"]))
                                poruka.PredavacID = Convert.ToInt32(dataReader["PredavacID"]);

                            poruka.SadrzajPoruke = Convert.ToString(dataReader["SadrzajPoruke"]);

                            if (!DBNull.Value.Equals(dataReader["StudentID"]))
                                poruka.StudentID = Convert.ToInt32(dataReader["StudentID"]);

                            if (!DBNull.Value.Equals(dataReader["TimID"]))
                                poruka.TimID = Convert.ToInt32(dataReader["TimID"]);

                            poruka.VidljivostPoruke = Convert.ToString(dataReader["VidljivostPoruke"]);
                            poruka.VrstaPoruke = Convert.ToString(dataReader["VrstaPoruke"]);
                            poruka.NotifikacijaID = Convert.ToInt32(dataReader["NotifikacijaID"]);
                        }
                    }

                    if (poruka == null)
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Error 404 Found");
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error retrieving data");
                }
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:63284/");
                client.DefaultRequestHeaders.Accept.Clear();
                var authorization = Request.Headers.GetValues("Authorization").FirstOrDefault();
                var token = authorization.Split(null)[1];
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("api-student/student/" + poruka.StudentID);

                if (response.IsSuccessStatusCode)
                {
                    var student = await response.Content.ReadAsAsync<NotifikovanStudentVO>();
                    poruka.Student = student;
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
                HttpResponseMessage response = await client.GetAsync("api-predavac/predavac/" + poruka.PredavacID);

                if (response.IsSuccessStatusCode)
                {
                    var predavac = await response.Content.ReadAsAsync<PredavacPosiljalacVO>();
                    poruka.Predavac = predavac;
                }
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:63280/");
                client.DefaultRequestHeaders.Accept.Clear();
                var authorization = Request.Headers.GetValues("Authorization").FirstOrDefault();
                var token = authorization.Split(null)[1];
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("api-tim/tim/" + poruka.TimID);

                if (response.IsSuccessStatusCode)
                {
                    var tim = await response.Content.ReadAsAsync<NotifikovanTimVO>();
                    poruka.Tim = tim;
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, poruka);
        }

        // POST: api/Poruka
        [HttpPost, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public HttpResponseMessage CreatePoruka([FromBody]Poruka poruka)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("INSERT INTO Poruka.Poruka VALUES(@SadrzajPoruke, @VrstaPoruke, @VidljivostPoruke, @PredavacID, @TimID, @StudentID, @NotifikacijaID)", connection);        
                sqlCmd.Parameters.AddWithValue("SadrzajPoruke", poruka.SadrzajPoruke);
                sqlCmd.Parameters.AddWithValue("VrstaPoruke", poruka.VrstaPoruke);
                sqlCmd.Parameters.AddWithValue("VidljivostPoruke", poruka.VidljivostPoruke);
                sqlCmd.Parameters.AddWithValue("PredavacID", poruka.PredavacID);
                sqlCmd.Parameters.AddWithValue("TimID", poruka.TimID);
                sqlCmd.Parameters.AddWithValue("StudentID", poruka.StudentID);
                sqlCmd.Parameters.AddWithValue("NotifikacijaID", poruka.NotifikacijaID);

                try
                {
                    connection.Open();
                    int i = sqlCmd.ExecuteNonQuery();
                    SqlCommand sqlPoruka = new SqlCommand("SELECT TOP 1 * FROM Poruka.Poruka ORDER BY PorukaID DESC", connection);
                    Poruka last = new Poruka();

                    using (SqlDataReader porukaRead = sqlPoruka.ExecuteReader())
                    {
                        while (porukaRead.Read())
                        {
                            last.PorukaID = Convert.ToInt32(porukaRead["PorukaID"]);

                            if (!DBNull.Value.Equals(porukaRead["PredavacID"]))
                                last.PredavacID = Convert.ToInt32(porukaRead["PredavacID"]);

                            last.SadrzajPoruke = Convert.ToString(porukaRead["SadrzajPoruke"]);

                            if (!DBNull.Value.Equals(porukaRead["StudentID"]))
                                last.StudentID = Convert.ToInt32(porukaRead["StudentID"]);

                            if (!DBNull.Value.Equals(porukaRead["TimID"]))
                                last.TimID = Convert.ToInt32(porukaRead["TimID"]);

                            last.VidljivostPoruke = Convert.ToString(porukaRead["VidljivostPoruke"]);
                            last.VrstaPoruke = Convert.ToString(porukaRead["VrstaPoruke"]);
                            last.NotifikacijaID = Convert.ToInt32(porukaRead["NotifikacijaID"]);
                        }
                    }

                    var response = Request.CreateResponse(HttpStatusCode.Created, last);
                    response.Headers.Location = new Uri(Request.RequestUri + "/" + last.PorukaID);
                    return response;
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error 404 Bad request");
                }
            }
        }

        // PUT: api/Poruka/5
        [HttpPut, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public HttpResponseMessage UpdatePoruka([FromBody]Poruka poruka)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("UPDATE Poruka.Poruka SET PredavacID=@PredavacID, SadrzajPoruke=@SadrzajPoruke, StudentID=@StudentID, TimID=@TimID, VidljivostPoruke=@VidljivostPoruke, VrstaPoruke=@VrstaPoruke, NotifikacijaID=@NotifikacijaID WHERE PorukaID=@id"))
                    {
                        connection.Open();
                        sqlCmd.Connection = connection;
                        sqlCmd.Parameters.AddWithValue("PredavacID", poruka.PredavacID);
                        sqlCmd.Parameters.AddWithValue("SadrzajPoruke", poruka.SadrzajPoruke);
                        sqlCmd.Parameters.AddWithValue("StudentID", poruka.StudentID);
                        sqlCmd.Parameters.AddWithValue("TimID", poruka.TimID);
                        sqlCmd.Parameters.AddWithValue("VidljivostPoruke", poruka.VidljivostPoruke);
                        sqlCmd.Parameters.AddWithValue("VrstaPoruke", poruka.VrstaPoruke);
                        sqlCmd.Parameters.AddWithValue("NotifikacijaID", poruka.NotifikacijaID);
                        sqlCmd.Parameters.AddWithValue("id", poruka.PorukaID);

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

        // DELETE: api/Poruka/5
        [HttpDelete, Route("{id}")]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public HttpResponseMessage DeletePoruka(int id)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("DELETE FROM Poruka.Poruka where PorukaID=@id"))
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
