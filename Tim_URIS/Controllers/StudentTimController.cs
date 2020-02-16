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
    [RoutePrefix("api-tim/student-tim")]
    public class StudentTimController : ApiController
    {
        public string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

        // GET: api/StudentTim
        [Route("")]
        [HttpGet]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        [Authorize(Roles = RolesConst.ROLE_Admin)]
        public HttpResponseMessage GetStudentTim()
        {
            List<StudentTim> result = new List<StudentTim>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Tim.StudentTim", connection)
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
                            var studentTim = new StudentTim
                            {
                                StudentTimID = Convert.ToInt32(dataReader["StudentTimID"]),
                                TimID = Convert.ToInt32(dataReader["TimID"]), 
                                StudentID = Convert.ToInt32(dataReader["StudentID"])
                            };

                            result.Add(studentTim);
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

        // GET: api/StudentTim/5
        [HttpGet, Route("{id}")]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public async Task<HttpResponseMessage> GetByIDAsync(int id)
        {
            StudentTimWithValueObject studentTim = null;
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Tim.StudentTim where StudentTimID=" + id, connection)
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
                            studentTim = new StudentTimWithValueObject
                            {
                                StudentTimID = Convert.ToInt32(dataReader["StudentTimID"]),
                                TimID = Convert.ToInt32(dataReader["TimID"]),
                                StudentID = Convert.ToInt32(dataReader["StudentID"])
                            };
                        }
                    }

                    if (studentTim == null)
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
                HttpResponseMessage response = await client.GetAsync("api-student/student/" + studentTim.StudentID);

                if (response.IsSuccessStatusCode)
                {
                    var student = await response.Content.ReadAsAsync<StudentInfoVO>();
                    studentTim.Student = student;
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, studentTim);
        }

        // POST: api/StudentTim
        [HttpPost, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public HttpResponseMessage CreateStudentTim([FromBody]StudentTim studentTim)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("INSERT INTO Tim.StudentTim VALUES(@TimID, @StudentID)", connection);
                sqlCmd.Parameters.AddWithValue("TimID", studentTim.TimID);
                sqlCmd.Parameters.AddWithValue("StudentID", studentTim.StudentID);

                try
                {
                    connection.Open();
                    int i = sqlCmd.ExecuteNonQuery();
                    SqlCommand sqlStudentTim = new SqlCommand("SELECT TOP 1 * FROM Tim.StudentTim ORDER BY StudentTimID DESC", connection);
                    StudentTim last = new StudentTim();
                    using (SqlDataReader studentTimRead = sqlStudentTim.ExecuteReader())
                    {
                        while (studentTimRead.Read())
                        {
                            last.StudentTimID = Convert.ToInt32(studentTimRead["StudentTimID"]);
                            last.TimID = Convert.ToInt32(studentTimRead["TimID"]);
                            last.StudentID = Convert.ToInt32(studentTimRead["StudentID"]);
                        }
                    }

                    var response = Request.CreateResponse(HttpStatusCode.Created, last);
                    response.Headers.Location = new Uri(Request.RequestUri + "/" + last.StudentTimID);
                    return response;
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error 404 Bad request");
                }
            }
        }

        // PUT: api/StudentTim/5
        [HttpPut, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public HttpResponseMessage UpdateStudentTim([FromBody]StudentTim studentTim)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("UPDATE Tim.StudentTim SET StudentID=@StudentID, TimID=@TimID WHERE StudentTimID=@id"))
                    {
                        connection.Open();
                        sqlCmd.Connection = connection;
                        sqlCmd.Parameters.AddWithValue("StudentID", studentTim.StudentID);
                        sqlCmd.Parameters.AddWithValue("TimID", studentTim.TimID);
                        sqlCmd.Parameters.AddWithValue("id", studentTim.StudentTimID);
                        sqlCmd.ExecuteNonQuery();
                    }
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error 400 Bad Request");
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, "Updated Successfully");
        }

        // DELETE: api/StudentTim/5
        [HttpDelete, Route("{id}")]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public HttpResponseMessage DeleteStudentTim(int id)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("DELETE FROM Tim.StudentTim where StudentTimID=@id"))
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
