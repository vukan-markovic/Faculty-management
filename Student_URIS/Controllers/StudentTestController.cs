using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Student_URIS.Models;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using URIS_v10.Filters;
using Student_URIS.AuthFilters;

namespace Student_URIS.Controllers
{
    [RoutePrefix("api-student/student-test")]
    public class StudentTestController : ApiController
    {
        public string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

        // GET: api/StudentTest
        [Route("")]
        [HttpGet]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        [Authorize(Roles = RolesConst.ROLE_Admin)]
        public HttpResponseMessage GetStudentTest()
        {
            List<StudentTest> result = new List<StudentTest>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Student.StudentTest", connection)
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
                            var studentTest = new StudentTest
                            {
                                StudentTestID = Convert.ToInt32(dataReader["StudentTestID"]),
                                TestID = Convert.ToInt32(dataReader["TestID"]),
                                StudentID = Convert.ToInt32(dataReader["StudentID"])
                            };

                            result.Add(studentTest);
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

        // GET: api/StudentTest/5
        [HttpGet, Route("{id}")]
        [ClientCacheControlFilter(ClientCacheControl.Private, 5)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public HttpResponseMessage GetByID(int id)
        {
            StudentTest studentTest = null;

            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("select * from Student.StudentTest where StudentTestID=" + id, connection)
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
                            studentTest = new StudentTest
                            {
                                StudentTestID = Convert.ToInt32(dataReader["StudentTestID"]),
                                TestID = Convert.ToInt32(dataReader["TestID"]),
                                StudentID = Convert.ToInt32(dataReader["StudentID"])
                            };
                        }
                    }

                    if (studentTest == null)
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Error 404 Found");
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error retrieving data");
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, studentTest);
        }

        // POST: api/StudentTest
        [HttpPost, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public HttpResponseMessage CreateStudentTest([FromBody]StudentTest studentTest)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("INSERT INTO Student.StudentTest VALUES(@TestID, @StudentID)", connection);
                sqlCmd.Parameters.AddWithValue("TestID", studentTest.TestID);
                sqlCmd.Parameters.AddWithValue("StudentID", studentTest.StudentID);

                try
                {
                    connection.Open();
                    int i = sqlCmd.ExecuteNonQuery();
                    SqlCommand sqlStudentTest = new SqlCommand("SELECT TOP 1 * FROM Student.StudentTest ORDER BY StudentTestID DESC", connection);
                    StudentTest last = new StudentTest();
                    using (SqlDataReader studentTestRead = sqlStudentTest.ExecuteReader())
                    {
                        while (studentTestRead.Read())
                        {
                            last.StudentTestID = Convert.ToInt32(studentTestRead["StudentTestID"]);
                            last.TestID = Convert.ToInt32(studentTestRead["TestID"]);
                            last.StudentID = Convert.ToInt32(studentTestRead["StudentID"]);
                        }
                    }

                    var response = Request.CreateResponse(HttpStatusCode.Created, last);
                    response.Headers.Location = new Uri(Request.RequestUri + "/" + last.TestID);
                    return response;
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error 404 Bad request");
                }
            }
        }

        // PUT: api/StudentTest/5
        [HttpPut, Route("")]
        [ValidateModelState(BodyRequired = true)]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public HttpResponseMessage UpdateStudentTest([FromBody]StudentTest studentTest)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("UPDATE Student.StudentTest SET TestID=@TestID, StudentID=@StudentID WHERE StudentTestID=@id"))
                    {
                        connection.Open();
                        sqlCmd.Connection = connection;
                        sqlCmd.Parameters.AddWithValue("TestID", studentTest.TestID);
                        sqlCmd.Parameters.AddWithValue("StudentID", studentTest.StudentID);
                        sqlCmd.Parameters.AddWithValue("id", studentTest.StudentTestID);
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

        // DELETE: api/StudentTest/5
        [HttpDelete, Route("{id}")]
        [Authorize(Roles = RolesConst.ROLE_Admin_Student_Predavac)]
        public HttpResponseMessage DeleteStudentTest(int id)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var sqlCmd = new SqlCommand("DELETE FROM Student.StudentTest where StudentTestID=@id"))
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
