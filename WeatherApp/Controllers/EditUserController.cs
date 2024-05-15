using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WeatherApp.Models;

namespace WeatherApp.Controllers
{
    [EnableCors("*", "*", "*")]

    public class EditUserController : ApiController
    {
        private readonly string _connectionString = "Data Source=DESKTOP-IRBSTH0\\SQLEXPRESS01;Initial Catalog=WeatherApp;Integrated Security=True;Connect Timeout=30;Encrypt=False";

        [HttpPut]
        public IHttpActionResult EditUserDetails(int userId, User userDetails)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    // Update user details
                    using (var cmdUpdate = new SqlCommand("UPDATE Registration SET name = @Name, age = @Age, gender = @Gender WHERE id = @UserId", conn))
                    {
                        cmdUpdate.Parameters.AddWithValue("@Name", userDetails.name);
                        cmdUpdate.Parameters.AddWithValue("@Age", userDetails.age);
                        cmdUpdate.Parameters.AddWithValue("@Gender", userDetails.gender);
                        cmdUpdate.Parameters.AddWithValue("@UserId", userId);

                        int rowsAffected = cmdUpdate.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            // User details updated successfully
                            return Ok("User details updated");
                        }
                        else
                        {
                            // Failed to update user details
                            return InternalServerError(new Exception("Failed to update user details"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                return InternalServerError(ex);
            }
        }


    }
}
