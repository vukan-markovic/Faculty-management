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
using Poruka_URIS.AuthFilters;
using Poruka_URIS.Models;
using URIS_v10.Filters;

namespace Poruka_URIS.Controllers
{
    [RoutePrefix("api-poruka/student-poruka-primalac")]
    public class StudentPorukaPrimalacController : ApiController
    {
        public string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

        // GET: api/StudentPorukaPrimalac
        [Route("")]
        [HttpGet]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        [Authorize(Roles = RolesConst.ROLE_Admin)]
        public HttpResponseMessage GetStudentPorukaPrimalac()
        {
            List<StudentPorukaPrimalac> result = new List<StudentPorukaPrimalac>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Poruka.StudentPorukaPrimalac", connection)
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
                            var studentPorukaPrimalac = new StudentPorukaPrimalac
                            {
                                StudentPorukaPrimalacID = Convert.ToInt32(dataReader["StudentPorukaPrimalacID"]),
                                PorukaID = Convert.ToInt32(dataReader["PorukaID"]),
                                StudentID = Convert.ToInt32(dataReader["StudentID"])
                            };

                            result.Add(studentPorukaPrimalac);
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

        // GET: api/StudentPorukaPrimalac/5
        [HttpGet, Route("{id}")]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public async Task<HttpResponseMessage> GetByIDAsync(int id)
        {
            StudentPorukaPrimalacWithValueObject studentPorukaPrimalac = null;
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Poruka.StudentPorukaPrimalac where StudentPorukaPrimalacID=" + id, connection)
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
                            studentPorukaPrimalac = new StudentPorukaPrimalacWithValueObject
                            {
                                StudentPorukaPrimalacID = Convert.ToInt32(dataReader["StudentPorukaPrimalacID"]),
                                PorukaID = Convert.ToInt32(dataReader["PorukaID"]),
                                StudentID = Convert.ToInt32(dataReader["StudentID"]),
                            };
                        }
                    }

                    if (studentPorukaPrimalac == null)
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
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("api-student/student/" + studentPorukaPrimalac.StudentID);

                if (response.IsSuccessStatusCode)
                {
                    var student = await response.Content.ReadAsAsync<NotifikovanStudentVO>();
                    studentPorukaPrimalac.Student = student;
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, studentPorukaPrimalac);
        }

        // POST: api/StudentPorukaPrimalac
        [HttpPost, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public HttpResponseMessage CreateStudentPorukaPrimalac([FromBody]StudentPorukaPrimalac studentPorukaPrimalac)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("INSERT INTO Poruka.StudentPorukaPrimalac VALUES(@PorukaID, @StudentID)", connection);
                sqlCmd.Parameters.AddWithValue("PorukaID", studentPorukaPrimalac.PorukaID);
                sqlCmd.Parameters.AddWithValue("StudentID", studentPorukaPrimalac.StudentID);

                try
                {
                    connection.Open();
                    int i = sqlCmd.ExecuteNonQuery();
                    SqlCommand sqlstudentPorukaPrimalac = new SqlCommand("SELECT TOP 1 * FROM Poruka.StudentPorukaPrimalac ORDER BY StudentPorukaPrimalacID DESC", connection);
                    StudentPorukaPrimalac last = new StudentPorukaPrimalac();
                    using (SqlDataReader studentPorukaPrimalacRead = sqlstudentPorukaPrimalac.ExecuteReader())
                    {
                        while (studentPorukaPrimalacRead.Read())
                        {
                            last.StudentPorukaPrimalacID = Convert.ToInt32(studentPorukaPrimalacRead["StudentPorukaPrimalacID"]);
                            last.PorukaID = Convert.ToInt32(studentPorukaPrimalacRead["PorukaID"]);
                            last.StudentID = Convert.ToInt32(studentPorukaPrimalacRead["StudentID"]);
                        }
                    }

                    var response = Request.CreateResponse(HttpStatusCode.Created, last);
                    response.Headers.Location = new Uri(Request.RequestUri + "/" + last.StudentPorukaPrimalacID);
                    return response;
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error 404 Bad request");
                }
            }
        }

        // PUT: api/StudentPorukaPrimalac/5
        [HttpPut, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public HttpResponseMessage UpdateStudentPorukaPrimalac([FromBody]StudentPorukaPrimalac studentPorukaPrimalac)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("UPDATE Poruka.StudentPorukaPrimalac SET PorukaID=@PorukaID, StudentID=@StudentID WHERE StudentPorukaPrimalacID=@id"))
                    {
                        connection.Open();
                        sqlCmd.Connection = connection;
                        sqlCmd.Parameters.AddWithValue("PorukaID", studentPorukaPrimalac.PorukaID);
                        sqlCmd.Parameters.AddWithValue("StudentID", studentPorukaPrimalac.StudentID);
                        sqlCmd.Parameters.AddWithValue("id", studentPorukaPrimalac.StudentPorukaPrimalacID);
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

        // DELETE: api/StudentPorukaPrimalac/5
        [HttpDelete, Route("{id}")]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public HttpResponseMessage DeleteStudentPorukaPrimalac(int id)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("DELETE FROM Poruka.StudentPorukaPrimalac where StudentPorukaPrimalacID=@id"))
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
