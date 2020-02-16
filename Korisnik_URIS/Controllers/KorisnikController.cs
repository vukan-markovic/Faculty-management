using Korisnik_URIS.AuthFilters;
using Korisnik_URIS.Filters;
using Korisnik_URIS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Korisnik_URIS.Controllers
{
    [RoutePrefix("api-korisnik/korisnik")]
    public class KorisnikController : ApiController
    {
        public string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

        // GET: api/Korisnik
        [Route("")]
        [HttpGet]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        [Authorize(Roles = RolesConst.ROLE_Admin)]
        public HttpResponseMessage GetKorisnici()
        {
            List<Korisnik> result = new List<Korisnik>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Korisnik.Korisnik", connection)
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
                            var korisnik = new Korisnik
                            {
                                KorisnikID = Convert.ToInt32(dataReader["KorisnikID"]),
                                Username = Convert.ToString(dataReader["Username"]),
                                Sifra = Convert.ToString(dataReader["Sifra"]),
                                Ime = Convert.ToString(dataReader["Ime"]),
                                Prezime = Convert.ToString(dataReader["Prezime"]),
                                Email = Convert.ToString(dataReader["Email"]),
                                TrenutnaUloga = Convert.ToString(dataReader["TrenutnaUloga"])
                            };

                            result.Add(korisnik);
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

        // GET: api/Korisnik/5
        [HttpGet, Route("{id}")]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public HttpResponseMessage GetByID(int id)
        {
            Korisnik korisnik = null;
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Korisnik.Korisnik where KorisnikID=" + id, connection)
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
                            korisnik = new Korisnik
                            {
                                KorisnikID = Convert.ToInt32(dataReader["KorisnikID"]),
                                Username = Convert.ToString(dataReader["Username"]),
                                Sifra = Convert.ToString(dataReader["Sifra"]),
                                Ime = Convert.ToString(dataReader["Ime"]),
                                Prezime = Convert.ToString(dataReader["Prezime"]),
                                Email = Convert.ToString(dataReader["Email"]),
                                TrenutnaUloga = Convert.ToString(dataReader["TrenutnaUloga"])
                            };
                        }
                    }

                    if (korisnik == null)
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Error 404 Found");
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error retrieving data");
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, korisnik);
        }

        // POST: api/Korisnik
        [HttpPost, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public HttpResponseMessage CreateKorisnik([FromBody]Korisnik korisnik)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("INSERT INTO Korisnik.Korisnik VALUES(@Username, @Sifra, @Ime, @Prezime, @Email, @TrenutnaUloga)", connection);
                sqlCmd.Parameters.AddWithValue("Username", korisnik.Username);
                sqlCmd.Parameters.AddWithValue("Sifra", korisnik.Sifra);
                sqlCmd.Parameters.AddWithValue("Ime", korisnik.Ime);
                sqlCmd.Parameters.AddWithValue("Prezime", korisnik.Prezime);
                sqlCmd.Parameters.AddWithValue("Email", korisnik.Email);
                sqlCmd.Parameters.AddWithValue("TrenutnaUloga", korisnik.TrenutnaUloga);

                try
                {
                    connection.Open();
                    sqlCmd.ExecuteNonQuery();
                    SqlCommand sqlKorisnik = new SqlCommand("SELECT TOP 1 * FROM Korisnik.Korisnik ORDER BY KorisnikID DESC", connection);
                    Korisnik last = new Korisnik();
                    using (var korisnikRead = sqlKorisnik.ExecuteReader())
                    {
                        while (korisnikRead.Read())
                        {
                            last.KorisnikID = Convert.ToInt32(korisnikRead["KorisnikID"]);
                            last.Username = Convert.ToString(korisnikRead["Username"]);
                            last.Sifra = Convert.ToString(korisnikRead["Sifra"]);
                            last.Ime = Convert.ToString(korisnikRead["Ime"]);
                            last.Prezime = Convert.ToString(korisnikRead["Prezime"]);
                            last.Email = Convert.ToString(korisnikRead["Email"]);
                            last.TrenutnaUloga = Convert.ToString(korisnikRead["TrenutnaUloga"]);
                        }
                    }

                    var response = Request.CreateResponse(HttpStatusCode.Created, last);
                    response.Headers.Location = new Uri(Request.RequestUri + "/" + last.KorisnikID);
                    return response;
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error 404 Bad request");
                }
            }
        }

        // PUT: api/Korisnik/
        [HttpPut, Route("")]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        [ValidateModelState(BodyRequired = true)]
        public HttpResponseMessage UpdateKorisnik([FromBody]Korisnik korisnik)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("UPDATE Korisnik.Korisnik SET Username=@Username, Sifra=@Sifra, Ime=@Ime, Prezime=@Prezime, Email=@Email, TrenutnaUloga=@TrenutnaUloga WHERE KorisnikID=@id"))
                    {
                        connection.Open();
                        sqlCmd.Connection = connection;
                        sqlCmd.Parameters.AddWithValue("Username", korisnik.Username);
                        sqlCmd.Parameters.AddWithValue("Sifra", korisnik.Sifra);
                        sqlCmd.Parameters.AddWithValue("Ime", korisnik.Ime);
                        sqlCmd.Parameters.AddWithValue("Prezime", korisnik.Prezime);
                        sqlCmd.Parameters.AddWithValue("Email", korisnik.Email);
                        sqlCmd.Parameters.AddWithValue("TrenutnaUloga", korisnik.TrenutnaUloga);
                        sqlCmd.Parameters.AddWithValue("id", korisnik.KorisnikID);
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

        // DELETE: api/Korisnik/5
        [HttpDelete, Route("{id}")]
        [Authorize(Roles = RolesConst.ROLE_Admin)]
        public HttpResponseMessage DeleteKorisnik(int id)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("DELETE FROM Korisnik.Korisnik where KorisnikID=@id"))
                    {
                        connection.Open();
                        sqlCmd.Connection = connection;
                        sqlCmd.Parameters.AddWithValue("id", id);
                        int rowAffected = sqlCmd.ExecuteNonQuery();
                        if (rowAffected == 0)
                            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Error 404 Not Found");
                    }
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error 404 Bad request",ex);
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, "Deleted Successfully");
        }
    }
}
