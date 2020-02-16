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
using Tim_URIS.AuthFilters;
using Tim_URIS.Models;
using URIS_v10.Filters;

namespace Tim_URIS.Controllers
{
    [RoutePrefix("api-tim/student-sastanak")]
    public class StudentSastanakController : ApiController
    {
        public string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

        // GET: api/StudentSastanak
        [Route("")]
        [HttpGet]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        [Authorize(Roles = RolesConst.ROLE_Admin)]
        public HttpResponseMessage GetStudentSastanak()
        {
            List<StudentSastanak> result = new List<StudentSastanak>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Tim.StudentSastanak", connection)
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
                            var studentSastanak = new StudentSastanak
                            {
                                StudentSastanakID = Convert.ToInt32(dataReader["StudentSastanakID"]),
                                SastanakID = Convert.ToInt32(dataReader["SastanakID"]), 
                                StudentID = Convert.ToInt32(dataReader["StudentID"])
                            };

                            result.Add(studentSastanak);
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

        // GET: api/StudentSastanak/5
        [HttpGet, Route("{id}")]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public async Task<HttpResponseMessage> GetByIDAsync(int id)
        {
            StudentSastanakWithValueObject studentSastanak = null;

            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Tim.StudentSastanak where StudentSastanakID=" + id, connection)
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
                            studentSastanak = new StudentSastanakWithValueObject
                            {
                                StudentSastanakID = Convert.ToInt32(dataReader["StudentSastanakID"]),
                                SastanakID = Convert.ToInt32(dataReader["SastanakID"]),
                                StudentID = Convert.ToInt32(dataReader["StudentID"])
                            };
                        }
                    }

                    if (studentSastanak == null)
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
                HttpResponseMessage response = await client.GetAsync("api-student/student/" + studentSastanak.StudentID);

                if (response.IsSuccessStatusCode)
                {
                    var student = await response.Content.ReadAsAsync<StudentInfoVO>();
                    studentSastanak.Student = student;
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, studentSastanak);
        }

        // POST: api/StudentSastanak
        [HttpPost, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public HttpResponseMessage CreateStudentSastanak([FromBody]StudentSastanak studentSastanak)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("INSERT INTO Tim.StudentSastanak VALUES(@SastanakID, @StudentID)", connection);
                sqlCmd.Parameters.AddWithValue("SastanakID", studentSastanak.SastanakID);
                sqlCmd.Parameters.AddWithValue("StudentID", studentSastanak.StudentID);

                try
                {
                    connection.Open();
                    int i = sqlCmd.ExecuteNonQuery();
                    SqlCommand sqlUniverzitet = new SqlCommand("SELECT TOP 1 * FROM Tim.StudentSastanak ORDER BY StudentSastanakID DESC", connection);
                    StudentSastanak last = new StudentSastanak();
                    using (SqlDataReader studentSastanakRead = sqlUniverzitet.ExecuteReader())
                    {
                        while (studentSastanakRead.Read())
                        {
                            studentSastanak = new StudentSastanak
                            {
                                StudentSastanakID = Convert.ToInt32(studentSastanakRead["StudentSastanakID"]),
                                SastanakID = Convert.ToInt32(studentSastanakRead["SastanakID"]),
                                StudentID = Convert.ToInt32(studentSastanakRead["StudentID"])
                            };
                        }
                    }

                    var response = Request.CreateResponse(HttpStatusCode.Created, last);
                    response.Headers.Location = new Uri(Request.RequestUri + "/" + last.StudentSastanakID);
                    return response;
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error 404 Bad request");
                }
            }
        }

        // PUT: api/StudentSastanak/5
        [HttpPut, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public HttpResponseMessage UpdateStudentSastanak([FromBody]StudentSastanak studentSastanak)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("UPDATE Tim.StudentSastanak SET StudentID=@StudentID, SastanakID=@SastanakID WHERE StudentSastanakID=@id"))
                    {
                        connection.Open();
                        sqlCmd.Connection = connection;
                        sqlCmd.Parameters.AddWithValue("StudentID", studentSastanak.StudentID);
                        sqlCmd.Parameters.AddWithValue("SastanakID", studentSastanak.SastanakID);
                        sqlCmd.Parameters.AddWithValue("id", studentSastanak.StudentSastanakID);
                        int rowAffected = sqlCmd.ExecuteNonQuery();
                        if (rowAffected == 0)
                            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Error 404 Not Found");
                    }
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error 400 Bad Request");
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, "Updated Successfully");
        }

        // DELETE: api/StudentSastanak/5
        [HttpDelete, Route("{id}")]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public HttpResponseMessage DeleteStudentSastanak(int id)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("DELETE FROM Tim.StudentSastanak where StudentSastanakID=@id"))
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
