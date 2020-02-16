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
using Predavac_URIS.AuthFilters;
using Predavac_URIS.Models;
using URIS_v10.Filters;

namespace Predavac_URIS.Controllers
{
    [RoutePrefix("api-predavac/predavac")]
    public class PredavacController : ApiController
    {
        public string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

        // GET: api/Predavac
        [Route("")]
        [HttpGet]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public HttpResponseMessage GetPredavaci()
        {
            List<Predavac> result = new List<Predavac>();

            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Predavac.Predavac", connection)
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
                            var predavac = new Predavac
                            {
                                PredavacID = Convert.ToInt32(dataReader["PredavacID"]),
                                DatumRodjenjaPredavaca = Convert.ToDateTime(dataReader["DatumRodjenjaPredavaca"]),
                                MestoRodjenjaPredavaca = Convert.ToString(dataReader["MestoRodjenjaPredavaca"]),
                                KatedraPredavaca = Convert.ToString(dataReader["KatedraPredavaca"]),
                                ZvanjePredavacaID = Convert.ToInt32(dataReader["ZvanjePredavacaID"]),
                                KorisnikID = Convert.ToInt32(dataReader["KorisnikID"]), 
                                DepartmanID = Convert.ToInt32(dataReader["DepartmanID"])
                            };
                            
                            result.Add(predavac);
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

        // GET: api/Predavac/5
        [HttpGet, Route("{id}")]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public async Task<HttpResponseMessage> GetByIDAsync(int id)
        {
            PredavacWithValueObject predavac = null;

            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Predavac.Predavac where PredavacID=" + id, connection)
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
                            predavac = new PredavacWithValueObject
                            {
                                PredavacID = Convert.ToInt32(dataReader["PredavacID"]),
                                DatumRodjenjaPredavaca = Convert.ToDateTime(dataReader["DatumRodjenjaPredavaca"]),
                                MestoRodjenjaPredavaca = Convert.ToString(dataReader["MestoRodjenjaPredavaca"]),
                                KatedraPredavaca = Convert.ToString(dataReader["KatedraPredavaca"]),
                                ZvanjePredavacaID = Convert.ToInt32(dataReader["ZvanjePredavacaID"]),
                                KorisnikID = Convert.ToInt32(dataReader["KorisnikID"]),
                                DepartmanID = Convert.ToInt32(dataReader["DepartmanID"])
                            };          
                        }
                    }

                    if (predavac == null)
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Error 404 Found");
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error retrieving data");
                }
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:63288/");
                client.DefaultRequestHeaders.Accept.Clear();
                var authorization = Request.Headers.GetValues("Authorization").FirstOrDefault();
                var token = authorization.Split(null)[1];
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("api-korisnik/korisnik/" + predavac.KorisnikID);

                if (response.IsSuccessStatusCode)
                {
                    var korisnik = await response.Content.ReadAsAsync<KorisnikInfoVO>();
                    predavac.Korisnik = korisnik;
                }
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:63298/");
                client.DefaultRequestHeaders.Accept.Clear();
                var authorization = Request.Headers.GetValues("Authorization").FirstOrDefault();
                var token = authorization.Split(null)[1];
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("api-departman/departman/" + predavac.DepartmanID);

                if (response.IsSuccessStatusCode)
                {
                    var departman = await response.Content.ReadAsAsync<DepartmanVO>();
                    predavac.Departman = departman;
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, predavac);
        }

        // GET: api/Predavac/Departman/5
        [HttpGet, Route("departman/{id}")]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public HttpResponseMessage GetByDepartmanID(int id)
        {
            List<Predavac> result = new List<Predavac>();

            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Predavac.Predavac where DepartmanID=" + id, connection)
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
                            var predavac = new Predavac
                            {
                                PredavacID = Convert.ToInt32(dataReader["PredavacID"]),
                                DatumRodjenjaPredavaca = Convert.ToDateTime(dataReader["DatumRodjenjaPredavaca"]),
                                MestoRodjenjaPredavaca = Convert.ToString(dataReader["MestoRodjenjaPredavaca"]),
                                KatedraPredavaca = Convert.ToString(dataReader["KatedraPredavaca"]),
                                ZvanjePredavacaID = Convert.ToInt32(dataReader["ZvanjePredavacaID"]),
                                KorisnikID = Convert.ToInt32(dataReader["KorisnikID"]),
                                DepartmanID = Convert.ToInt32(dataReader["DepartmanID"])
                            };

                            result.Add(predavac);
                        }
                    }

                    if (result == null)
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Error 404 Found");
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error retrieving data");
                }
            }
    
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        // POST: api/Predavac
        [HttpPost, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin)]
        public HttpResponseMessage CreatePredavac([FromBody]Predavac predavac)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("INSERT INTO Predavac.Predavac VALUES(@DatumRodjenjaPredavaca, @KatedraPredavaca, @MestoRodjenjaPredavaca, @ZvanjePredavacaID, @KorisnikID, @DepartmanID)", connection);
                sqlCmd.Parameters.AddWithValue("DatumRodjenjaPredavaca", predavac.DatumRodjenjaPredavaca);
                sqlCmd.Parameters.AddWithValue("KatedraPredavaca", predavac.KatedraPredavaca);
                sqlCmd.Parameters.AddWithValue("MestoRodjenjaPredavaca", predavac.MestoRodjenjaPredavaca);
                sqlCmd.Parameters.AddWithValue("ZvanjePredavacaID", predavac.ZvanjePredavacaID);
                sqlCmd.Parameters.AddWithValue("KorisnikID", predavac.KorisnikID);     
                sqlCmd.Parameters.AddWithValue("DepartmanID", predavac.DepartmanID);

                try
                {
                    connection.Open();
                    int i = sqlCmd.ExecuteNonQuery();
                    SqlCommand sqlUniverzitet = new SqlCommand("SELECT TOP 1 * FROM Predavac.Predavac ORDER BY PredavacID DESC", connection);
                    Predavac last = new Predavac();
                    using (SqlDataReader predavacRead = sqlUniverzitet.ExecuteReader())
                    {
                        while (predavacRead.Read())
                        {
                            last.PredavacID = Convert.ToInt32(predavacRead["PredavacID"]);
                            last.DatumRodjenjaPredavaca = Convert.ToDateTime(predavacRead["DatumRodjenjaPredavaca"]);
                            last.MestoRodjenjaPredavaca = Convert.ToString(predavacRead["MestoRodjenjaPredavaca"]);
                            last.KatedraPredavaca = Convert.ToString(predavacRead["KatedraPredavaca"]);
                            last.ZvanjePredavacaID = Convert.ToInt32(predavacRead["ZvanjePredavacaID"]);
                            last.KorisnikID = Convert.ToInt32(predavacRead["KorisnikID"]);
                            last.DepartmanID = Convert.ToInt32(predavacRead["DepartmanID"]);
                        }
                    }

                    var response = Request.CreateResponse(HttpStatusCode.Created, last);
                    response.Headers.Location = new Uri(Request.RequestUri + "/" + last.PredavacID);
                    return response;
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error 404 Bad request");
                }
            }
        }

        // PUT: api/Predavac/5
        [HttpPut, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Predavac)]
        public HttpResponseMessage UpdatePredavac([FromBody]Predavac predavac)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("UPDATE Predavac.Predavac SET DatumRodjenjaPredavaca=@DatumRodjenjaPredavaca, KatedraPredavaca=@KatedraPredavaca, MestoRodjenjaPredavaca=@MestoRodjenjaPredavaca, ZvanjePredavacaID=@ZvanjePredavacaID, KorisnikID=@KorisnikID, DepartmanID=@DepartmanID WHERE PredavacID=@id"))
                    {
                        connection.Open();
                        sqlCmd.Connection = connection;
                        sqlCmd.Parameters.AddWithValue("DatumRodjenjaPredavaca", predavac.DatumRodjenjaPredavaca);
                        sqlCmd.Parameters.AddWithValue("KatedraPredavaca", predavac.KatedraPredavaca);
                        sqlCmd.Parameters.AddWithValue("MestoRodjenjaPredavaca", predavac.MestoRodjenjaPredavaca);
                        sqlCmd.Parameters.AddWithValue("ZvanjePredavacaID", predavac.ZvanjePredavacaID);
                        sqlCmd.Parameters.AddWithValue("KorisnikID", predavac.KorisnikID);
                        sqlCmd.Parameters.AddWithValue("DepartmanID", predavac.DepartmanID);
                        sqlCmd.Parameters.AddWithValue("id", predavac.PredavacID);
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

        // DELETE: api/Predavac/5
        [HttpDelete, Route("{id}")]
        [Authorize(Roles = RolesConst.ROLE_Admin)]
        public HttpResponseMessage DeletePredavac(int id)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("DELETE FROM Predavac.Predavac where PredavacID=@id"))
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