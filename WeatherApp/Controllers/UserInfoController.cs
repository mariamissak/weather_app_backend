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
    public class UserInfoController : ApiController
    {
        private readonly string _connectionString = "Data Source=DESKTOP-IRBSTH0\\SQLEXPRESS01;Initial Catalog=WeatherApp;Integrated Security=True;Connect Timeout=30;Encrypt=False";

        [HttpGet]
        public IHttpActionResult GetUserById(int userId)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    // Fetch the user details from the database based on the provided user ID
                    using (var cmdSelect = new SqlCommand("SELECT * FROM Registration WHERE id = @UserId", conn))
                    {
                        cmdSelect.Parameters.AddWithValue("@UserId", userId);
                        using (var reader = cmdSelect.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Create a User object with the fetched user details
                                User user = new User
                                {
                                    id = Convert.ToInt32(reader["id"]),
                                    name = reader["name"].ToString(),
                                    email = reader["email"].ToString(),
                                    password = reader["password"].ToString(),
                                    age = Convert.ToInt32(reader["age"]),
                                    gender = reader["gender"].ToString()[0]
                                };

                                // Return the user details
                                return Ok(user);
                            }
                            else
                            {
                                // User with provided ID not found
                                return NotFound();
                            }
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
