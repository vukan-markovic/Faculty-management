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
using Student_URIS.AuthFilters;
using Student_URIS.Models;
using URIS_v10.Filters;

namespace Student_URIS.Controllers
{
    [RoutePrefix("api-student/student")]
    public class StudentController : ApiController
    {
        public string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
        // GET: api/Student
        [Route("")]
        [HttpGet]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        [Authorize(Roles = RolesConst.ROLE_Admin)]
        public HttpResponseMessage GetStudenti()
        {
            List<Student> result = new List<Student>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Student.Student", connection)
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
                            var student = new Student
                            {
                                StudentID = Convert.ToInt32(dataReader["StudentID"]),
                                DatumRodjenjaStudenta = Convert.ToDateTime(dataReader["DatumRodjenjaStudenta"]),
                                MestoRodjenjaStudenta = Convert.ToString(dataReader["MestoRodjenjaStudenta"]),
                                BrojIndeksaStudenta = Convert.ToString(dataReader["BrojIndeksaStudenta"]),
                                KatedraStudenta = Convert.ToString(dataReader["KatedraStudenta"]),
                                StudijskiProgramStudenta = Convert.ToString(dataReader["StudijskiProgramStudenta"]),
                                VrstaStudija = Convert.ToString(dataReader["VrstaStudija"]),
                                StepenStudija = Convert.ToString(dataReader["StepenStudija"]),
                                GodinaStudija = Convert.ToString(dataReader["GodinaStudija"]),
                                RbrUpisaneGodine = Convert.ToInt32(dataReader["RbrUpisaneGodine"]),
                                GodinaUpisaFakulteta = Convert.ToInt32(dataReader["GodinaUpisaFakulteta"]),
                                NacinFinansiranja = Convert.ToString(dataReader["NacinFinansiranja"]),
                                SifraStudenta = Convert.ToString(dataReader["SifraStudenta"]),
                                KorisnikID = Convert.ToInt32(dataReader["KorisnikID"]),
                                DepartmanID = Convert.ToInt32(dataReader["DepartmanID"])
                            };

                            result.Add(student);
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

        // GET: api/Student/5
        [HttpGet, Route("{id}")]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public async Task<HttpResponseMessage> GetByIDAsync(int id)
        {
            StudentWithValueObjects student = null;
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Student.Student where StudentID=" + id, connection)
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
                            student = new StudentWithValueObjects
                            {
                                StudentID = Convert.ToInt32(dataReader["StudentID"]),
                                DatumRodjenjaStudenta = Convert.ToDateTime(dataReader["DatumRodjenjaStudenta"]),
                                MestoRodjenjaStudenta = Convert.ToString(dataReader["MestoRodjenjaStudenta"]),
                                BrojIndeksaStudenta = Convert.ToString(dataReader["BrojIndeksaStudenta"]),
                                KatedraStudenta = Convert.ToString(dataReader["KatedraStudenta"]),
                                StudijskiProgramStudenta = Convert.ToString(dataReader["StudijskiProgramStudenta"]),
                                VrstaStudija = Convert.ToString(dataReader["VrstaStudija"]),
                                StepenStudija = Convert.ToString(dataReader["StepenStudija"]),
                                GodinaStudija = Convert.ToString(dataReader["GodinaStudija"]),
                                RbrUpisaneGodine = Convert.ToInt32(dataReader["RbrUpisaneGodine"]),
                                GodinaUpisaFakulteta = Convert.ToInt32(dataReader["GodinaUpisaFakulteta"]),
                                NacinFinansiranja = Convert.ToString(dataReader["NacinFinansiranja"]),
                                SifraStudenta = Convert.ToString(dataReader["SifraStudenta"]),
                                KorisnikID = Convert.ToInt32(dataReader["KorisnikID"]),
                                DepartmanID = Convert.ToInt32(dataReader["DepartmanID"])
                            };
                        }
                    }

                    if (student == null)
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
                HttpResponseMessage response = await client.GetAsync("api-korisnik/korisnik/" + student.KorisnikID);

                if (response.IsSuccessStatusCode)
                {
                    var Korisnik = await response.Content.ReadAsAsync<KorisnikInfoVO>();
                    student.Korisnik = Korisnik;
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
                HttpResponseMessage response = await client.GetAsync("api-departman/departman/" + student.DepartmanID);

                if (response.IsSuccessStatusCode)
                {
                    var Departman = await response.Content.ReadAsAsync<DepartmanInfoVO>();
                    student.Departman = Departman;
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, student);
        }

        // POST: api/Student
        [HttpPost, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin)]
        public HttpResponseMessage CreateStudent([FromBody]Student student)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("INSERT INTO Student.Student VALUES(@DatumRodjenjaStudenta, @MestoRodjenjaStudenta, @BrojIndeksaStudenta, @KatedraStudenta, @StudijskiProgramStudenta, @RbrUpisaneGodine, @GodinaUpisaFakulteta, @SifraStudenta, @VrstaStudija, @StepenStudija, @GodinaStudija, @NacinFinansiranja, @KorisnikID, @DepartmanID)", connection);
                sqlCmd.Parameters.AddWithValue("DatumRodjenjaStudenta", student.DatumRodjenjaStudenta);
                sqlCmd.Parameters.AddWithValue("MestoRodjenjaStudenta", student.MestoRodjenjaStudenta);
                sqlCmd.Parameters.AddWithValue("BrojIndeksaStudenta", student.BrojIndeksaStudenta);
                sqlCmd.Parameters.AddWithValue("KatedraStudenta", student.KatedraStudenta);
                sqlCmd.Parameters.AddWithValue("StudijskiProgramStudenta", student.StudijskiProgramStudenta);
                sqlCmd.Parameters.AddWithValue("RbrUpisaneGodine", student.RbrUpisaneGodine);
                sqlCmd.Parameters.AddWithValue("GodinaUpisaFakulteta", student.GodinaUpisaFakulteta);
                sqlCmd.Parameters.AddWithValue("SifraStudenta", student.SifraStudenta);
                sqlCmd.Parameters.AddWithValue("VrstaStudija", student.VrstaStudija);
                sqlCmd.Parameters.AddWithValue("StepenStudija", student.StepenStudija);
                sqlCmd.Parameters.AddWithValue("GodinaStudija", student.GodinaStudija);
                sqlCmd.Parameters.AddWithValue("NacinFinansiranja", student.NacinFinansiranja);
                sqlCmd.Parameters.AddWithValue("KorisnikID", student.KorisnikID);
                sqlCmd.Parameters.AddWithValue("DepartmanID", student.DepartmanID);

                try
                {
                    connection.Open();
                    int i = sqlCmd.ExecuteNonQuery();
                    SqlCommand sqlStudent = new SqlCommand("SELECT TOP 1 * FROM Student.Student ORDER BY StudentID DESC", connection);
                    Student last = new Student();
                    using (SqlDataReader studentRead = sqlStudent.ExecuteReader())
                    {
                        while (studentRead.Read())
                        {
                            last.StudentID = Convert.ToInt32(studentRead["StudentID"]);
                            last.DatumRodjenjaStudenta = Convert.ToDateTime(studentRead["DatumRodjenjaStudenta"]);
                            last.MestoRodjenjaStudenta = Convert.ToString(studentRead["MestoRodjenjaStudenta"]);
                            last.BrojIndeksaStudenta = Convert.ToString(studentRead["BrojIndeksaStudenta"]);
                            last.KatedraStudenta = Convert.ToString(studentRead["KatedraStudenta"]);
                            last.StudijskiProgramStudenta = Convert.ToString(studentRead["StudijskiProgramStudenta"]);
                            last.VrstaStudija = Convert.ToString(studentRead["VrstaStudija"]);
                            last.StepenStudija = Convert.ToString(studentRead["StepenStudija"]);
                            last.GodinaStudija = Convert.ToString(studentRead["GodinaStudija"]);
                            last.RbrUpisaneGodine = Convert.ToInt32(studentRead["RbrUpisaneGodine"]);
                            last.GodinaUpisaFakulteta = Convert.ToInt32(studentRead["GodinaUpisaFakulteta"]);
                            last.NacinFinansiranja = Convert.ToString(studentRead["NacinFinansiranja"]);
                            last.SifraStudenta = Convert.ToString(studentRead["SifraStudenta"]);
                            last.KorisnikID = Convert.ToInt32(studentRead["KorisnikID"]);
                            last.DepartmanID = Convert.ToInt32(studentRead["DepartmanID"]);
                        }
                    }
                    var response = Request.CreateResponse(HttpStatusCode.Created, last);
                    response.Headers.Location = new Uri(Request.RequestUri + "/" + last.StudentID);
                    return response;
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error 404 Bad request");
                }
            }
        }

        // PUT: api/Student/5
        [HttpPut, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student)]
        public HttpResponseMessage UpdatePredmet([FromBody]Student student)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("UPDATE Student.Student SET DatumRodjenjaStudenta=@DatumRodjenjaStudenta, MestoRodjenjaStudenta=@MestoRodjenjaStudenta, BrojIndeksaStudenta=@BrojIndeksaStudenta, KatedraStudenta=@KatedraStudenta, StudijskiProgramStudenta=@StudijskiProgramStudenta, VrstaStudija=@VrstaStudija, StepenStudija=@StepenStudija, GodinaStudija=@GodinaStudija, RbrUpisaneGodine=@RbrUpisaneGodine, GodinaUpisaFakulteta=@GodinaUpisaFakulteta, NacinFinansiranja=@NacinFinansiranja, SifraStudenta=@SifraStudenta, KorisnikID=@KorisnikID, DepartmanID=@DepartmanID WHERE StudentID=@id"))
                    {
                        connection.Open();
                        sqlCmd.Connection = connection;
                        sqlCmd.Parameters.AddWithValue("DatumRodjenjaStudenta", student.DatumRodjenjaStudenta);
                        sqlCmd.Parameters.AddWithValue("MestoRodjenjaStudenta", student.MestoRodjenjaStudenta);
                        sqlCmd.Parameters.AddWithValue("BrojIndeksaStudenta", student.BrojIndeksaStudenta);
                        sqlCmd.Parameters.AddWithValue("KatedraStudenta", student.KatedraStudenta);
                        sqlCmd.Parameters.AddWithValue("StudijskiProgramStudenta", student.StudijskiProgramStudenta);
                        sqlCmd.Parameters.AddWithValue("VrstaStudija", student.VrstaStudija);
                        sqlCmd.Parameters.AddWithValue("StepenStudija", student.StepenStudija);
                        sqlCmd.Parameters.AddWithValue("GodinaStudija", student.GodinaStudija);
                        sqlCmd.Parameters.AddWithValue("RbrUpisaneGodine", student.RbrUpisaneGodine);
                        sqlCmd.Parameters.AddWithValue("GodinaUpisaFakulteta", student.GodinaUpisaFakulteta);
                        sqlCmd.Parameters.AddWithValue("NacinFinansiranja", student.NacinFinansiranja);
                        sqlCmd.Parameters.AddWithValue("SifraStudenta", student.SifraStudenta);
                        sqlCmd.Parameters.AddWithValue("KorisnikID", student.KorisnikID);
                        sqlCmd.Parameters.AddWithValue("DepartmanID", student.DepartmanID);
                        sqlCmd.Parameters.AddWithValue("id", student.StudentID);
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

        // DELETE: api/Student/5
        [HttpDelete, Route("{id}")]
        [Authorize(Roles = RolesConst.ROLE_Admin)]
        public HttpResponseMessage DeleteStudent(int id)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("DELETE FROM Student.Student where Student.StudentID=@id"))
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