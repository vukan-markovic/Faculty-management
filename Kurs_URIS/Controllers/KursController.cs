using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Script.Serialization;
using Kurs_URIS.AuthFilters;
using URIS_v10.Filters;
using Kurs_URIS.Models;
using System.Linq;

namespace Kurs_URIS.Controllers
{
    [RoutePrefix("api-kurs/kurs")]
    public class KursController : ApiController
    {
        public string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

        // GET: api/Kurs
        [Route("")]
        [HttpGet]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Predavac)]
        public HttpResponseMessage GetKursevi()
        {
            List<Kurs> result = new List<Kurs>();

            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Kurs.Kurs", connection)
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
                            var kurs = new Kurs
                            {
                                KursID = Convert.ToInt32(dataReader["KursID"]),
                                NazivKursa = Convert.ToString(dataReader["NazivKursa"]),
                                BrStudenata = Convert.ToInt32(dataReader["BrStudenata"]),
                                RasporedPredavanja = Convert.ToString(dataReader["RasporedPredavanja"]),
                                StudijskiProgram = Convert.ToString(dataReader["StudijskiProgram"]),
                                SkolskaGodina = Convert.ToString(dataReader["SkolskaGodina"]), 
                                TezinskiFaktorOcene = Convert.ToDouble(dataReader["TezinskiFaktorOcene"]), 
                                PolitikaUpisaOcena = Convert.ToString(dataReader["PolitikaUpisaOcena"]),
                                PolitikaUpisaDepartman = Convert.ToString(dataReader["PolitikaUpisaDepartman"]), 
                                PolitikaUpisaFCFS = Convert.ToString(dataReader["PolitikaUpisaFCFS"]), 
                                MinimalanBrStudenata = Convert.ToInt32(dataReader["MinimalanBrStudenata"]),
                                DefinicijaPreduslova = Convert.ToString(dataReader["DefinicijaPreduslova"]),
                                KriterijumZaPolaganje = Convert.ToString(dataReader["KriterijumZaPolaganje"]), 
                                Pravilo = Convert.ToString(dataReader["Pravilo"]), 
                                PredmetID = Convert.ToInt32(dataReader["PredmetID"])
                            };

                            result.Add(kurs);
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

        // GET: api/Kurs/5
        [HttpGet, Route("{id}")]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public async Task<HttpResponseMessage> GetByIDAsync(int id)
        {
            KursWithValueObject kurs = null;

            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Kurs.Kurs where KursID=" + id, connection)
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
                            kurs = new KursWithValueObject
                            {
                                KursID = Convert.ToInt32(dataReader["KursID"]),
                                NazivKursa = Convert.ToString(dataReader["NazivKursa"]),
                                BrStudenata = Convert.ToInt32(dataReader["BrStudenata"]),
                                RasporedPredavanja = Convert.ToString(dataReader["RasporedPredavanja"]),
                                StudijskiProgram = Convert.ToString(dataReader["StudijskiProgram"]),
                                SkolskaGodina = Convert.ToString(dataReader["SkolskaGodina"]),
                                TezinskiFaktorOcene = Convert.ToDouble(dataReader["TezinskiFaktorOcene"]),
                                PolitikaUpisaOcena = Convert.ToString(dataReader["PolitikaUpisaOcena"]),
                                PolitikaUpisaDepartman = Convert.ToString(dataReader["PolitikaUpisaDepartman"]),
                                PolitikaUpisaFCFS = Convert.ToString(dataReader["PolitikaUpisaFCFS"]),
                                MinimalanBrStudenata = Convert.ToInt32(dataReader["MinimalanBrStudenata"]),
                                DefinicijaPreduslova = Convert.ToString(dataReader["DefinicijaPreduslova"]),
                                KriterijumZaPolaganje = Convert.ToString(dataReader["KriterijumZaPolaganje"]),
                                Pravilo = Convert.ToString(dataReader["Pravilo"]),
                                PredmetID = Convert.ToInt32(dataReader["PredmetID"])
                            };
                        }
                    }

                    if (kurs == null)
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Error 404 Found");
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error retrieving data");
                }
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:10682/");
                client.DefaultRequestHeaders.Accept.Clear();
                var authorization = Request.Headers.GetValues("Authorization").FirstOrDefault();
                var token = authorization.Split(null)[1];
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("api-predmet/predmet/" + kurs.PredmetID);

                if (response.IsSuccessStatusCode)
                {
                    var Predmet = await response.Content.ReadAsAsync<PredmetVO>();
                    kurs.Predmet = Predmet;
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, kurs);
        }

        // POST: api/Kurs
        [HttpPost, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Predavac)]
        public HttpResponseMessage CreateKurs([FromBody]Kurs kurs)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("INSERT INTO Kurs.Kurs VALUES(@NazivKursa, @BrStudenata, @RasporedPredavanja, @StudijskiProgram, @SkolskaGodina, @TezinskiFaktorOcene, @PolitikaUpisaOcena, @PolitikaUpisaDepartman, @PolitikaUpisaFCFS, @MinimalanBrStudenata, @PredmetID, @DefinicijaPreduslova, @KriterijumZaPolaganje, @Pravilo)", connection);
                sqlCmd.Parameters.AddWithValue("NazivKursa", kurs.NazivKursa);
                sqlCmd.Parameters.AddWithValue("BrStudenata", kurs.BrStudenata);
                sqlCmd.Parameters.AddWithValue("RasporedPredavanja", kurs.RasporedPredavanja);
                sqlCmd.Parameters.AddWithValue("StudijskiProgram", kurs.StudijskiProgram);
                sqlCmd.Parameters.AddWithValue("SkolskaGodina", kurs.SkolskaGodina);
                sqlCmd.Parameters.AddWithValue("TezinskiFaktorOcene", kurs.TezinskiFaktorOcene);
                sqlCmd.Parameters.AddWithValue("PolitikaUpisaOcena", kurs.PolitikaUpisaOcena);
                sqlCmd.Parameters.AddWithValue("PolitikaUpisaDepartman", kurs.PolitikaUpisaDepartman);
                sqlCmd.Parameters.AddWithValue("PolitikaUpisaFCFS", kurs.PolitikaUpisaFCFS);
                sqlCmd.Parameters.AddWithValue("MinimalanBrStudenata", kurs.MinimalanBrStudenata);
                sqlCmd.Parameters.AddWithValue("PredmetID", kurs.PredmetID);
                sqlCmd.Parameters.AddWithValue("DefinicijaPreduslova", kurs.DefinicijaPreduslova);
                sqlCmd.Parameters.AddWithValue("KriterijumZaPolaganje", kurs.KriterijumZaPolaganje);
                sqlCmd.Parameters.AddWithValue("Pravilo", kurs.Pravilo);

                try
                {
                    connection.Open();
                    int i = sqlCmd.ExecuteNonQuery();
                    SqlCommand sqlUniverzitet = new SqlCommand("SELECT TOP 1 * FROM Kurs.Kurs ORDER BY KursID DESC", connection);
                    Kurs last = new Kurs();
                    using (SqlDataReader kursRead = sqlUniverzitet.ExecuteReader())
                    {
                        while (kursRead.Read())
                        {
                            last.KursID = Convert.ToInt32(kursRead["KursID"]);
                            last.NazivKursa = Convert.ToString(kursRead["NazivKursa"]);
                            last.BrStudenata = Convert.ToInt32(kursRead["BrStudenata"]);
                            last.RasporedPredavanja = Convert.ToString(kursRead["RasporedPredavanja"]);
                            last.StudijskiProgram = Convert.ToString(kursRead["StudijskiProgram"]);
                            last.SkolskaGodina = Convert.ToString(kursRead["SkolskaGodina"]);
                            last.TezinskiFaktorOcene = Convert.ToDouble(kursRead["TezinskiFaktorOcene"]);
                            last.PolitikaUpisaOcena = Convert.ToString(kursRead["PolitikaUpisaOcena"]);
                            last.PolitikaUpisaDepartman = Convert.ToString(kursRead["PolitikaUpisaDepartman"]);
                            last.PolitikaUpisaFCFS = Convert.ToString(kursRead["PolitikaUpisaFCFS"]);
                            last.MinimalanBrStudenata = Convert.ToInt32(kursRead["MinimalanBrStudenata"]);
                            last.DefinicijaPreduslova = Convert.ToString(kursRead["DefinicijaPreduslova"]);
                            last.KriterijumZaPolaganje = Convert.ToString(kursRead["KriterijumZaPolaganje"]);
                            last.Pravilo = Convert.ToString(kursRead["Pravilo"]);
                            last.PredmetID = Convert.ToInt32(kursRead["PredmetID"]);
                        }
                    }

                    var response = Request.CreateResponse(HttpStatusCode.Created, last);
                    response.Headers.Location = new Uri(Request.RequestUri + "/" + last.KursID);
                    return response;
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error 404 Bad request");
                }
            }
        }

        // PUT: api/Kurs/5
        [HttpPut, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Predavac)]
        public HttpResponseMessage UpdateKurs([FromBody]Kurs kurs)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("UPDATE Kurs.Kurs SET NazivKursa=@NazivKursa, BrStudenata=@BrStudenata, RasporedPredavanja=@RasporedPredavanja, StudijskiProgram=@StudijskiProgram, SkolskaGodina=@SkolskaGodina, TezinskiFaktorOcene=@TezinskiFaktorOcene, PolitikaUpisaOcena=@PolitikaUpisaOcena, PolitikaUpisaDepartman=@PolitikaUpisaDepartman, PolitikaUpisaFCFS=@PolitikaUpisaFCFS, MinimalanBrStudenata=@MinimalanBrStudenata, DefinicijaPreduslova=@DefinicijaPreduslova, KriterijumZaPolaganje=@KriterijumZaPolaganje, Pravilo=@Pravilo, PredmetID=@PredmetID WHERE KursID=@id"))
                    {
                        connection.Open();
                        sqlCmd.Connection = connection;
                        sqlCmd.Parameters.AddWithValue("NazivKursa", kurs.NazivKursa);
                        sqlCmd.Parameters.AddWithValue("BrStudenata", kurs.BrStudenata);
                        sqlCmd.Parameters.AddWithValue("RasporedPredavanja", kurs.RasporedPredavanja);
                        sqlCmd.Parameters.AddWithValue("StudijskiProgram", kurs.StudijskiProgram);
                        sqlCmd.Parameters.AddWithValue("SkolskaGodina", kurs.SkolskaGodina);
                        sqlCmd.Parameters.AddWithValue("TezinskiFaktorOcene", kurs.TezinskiFaktorOcene);
                        sqlCmd.Parameters.AddWithValue("PolitikaUpisaOcena", kurs.PolitikaUpisaOcena);
                        sqlCmd.Parameters.AddWithValue("PolitikaUpisaDepartman", kurs.PolitikaUpisaDepartman);
                        sqlCmd.Parameters.AddWithValue("PolitikaUpisaFCFS", kurs.PolitikaUpisaFCFS);
                        sqlCmd.Parameters.AddWithValue("MinimalanBrStudenata", kurs.MinimalanBrStudenata);
                        sqlCmd.Parameters.AddWithValue("DefinicijaPreduslova", kurs.DefinicijaPreduslova);
                        sqlCmd.Parameters.AddWithValue("KriterijumZaPolaganje", kurs.KriterijumZaPolaganje);
                        sqlCmd.Parameters.AddWithValue("Pravilo", kurs.Pravilo);
                        sqlCmd.Parameters.AddWithValue("PredmetID", kurs.PredmetID);
                        sqlCmd.Parameters.AddWithValue("id", kurs.KursID);

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

        // DELETE: api/Kurs/5
        [HttpDelete, Route("{id}")]
        [Authorize(Roles = RolesConst.ROLE_Admin_Predavac)]
        public HttpResponseMessage DeleteKurs(int id)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("DELETE FROM Kurs.Kurs where KursID=@id"))
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
