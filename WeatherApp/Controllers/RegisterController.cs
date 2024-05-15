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
    public class RegisterController : ApiController
    {
        private readonly string _connectionString = "Data Source=DESKTOP-IRBSTH0\\SQLEXPRESS01;Initial Catalog=WeatherApp;Integrated Security=True;Connect Timeout=30;Encrypt=False";
        [HttpPost]
        public IHttpActionResult Register(User user)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    // Check if user with provided email already exists
                    using (var cmdCheck = new SqlCommand("SELECT COUNT(*) FROM Registration WHERE email = @Email", conn))
                    {
                        cmdCheck.Parameters.AddWithValue("@Email", user.email);
                        int existingUserCount = (int)cmdCheck.ExecuteScalar();
                        if (existingUserCount > 0)
                        {
                            // User with provided email already exists
                            return BadRequest("User already exists");
                        }
                    }

                    // User doesn't exist, insert new user record
                    using (var cmdInsert = new SqlCommand("INSERT INTO Registration (name, email, password, age, gender) VALUES (@Name, @Email, @Password, @Age, @Gender); SELECT SCOPE_IDENTITY()", conn))
                    {
                        cmdInsert.Parameters.AddWithValue("@Name", user.name);
                        cmdInsert.Parameters.AddWithValue("@Email", user.email);
                        cmdInsert.Parameters.AddWithValue("@Password", user.password);
                        cmdInsert.Parameters.AddWithValue("@Age", user.age);
                        cmdInsert.Parameters.AddWithValue("@Gender", user.gender);

                        // Execute the insert command and get the newly inserted user's ID
                        int userId = Convert.ToInt32(cmdInsert.ExecuteScalar());

                        // Fetch the user details from the database
                        using (var cmdSelect = new SqlCommand("SELECT * FROM Registration WHERE id = @UserId", conn))
                        {
                            cmdSelect.Parameters.AddWithValue("@UserId", userId);
                            using (var reader = cmdSelect.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    // Create a User object with the fetched user details
                                    User registeredUser = new User
                                    {
                                        id = Convert.ToInt32(reader["id"]),
                                        name = reader["name"].ToString(),
                                        email = reader["email"].ToString(),
                                        password = reader["password"].ToString(),
                                        age = Convert.ToInt32(reader["age"]),
                                        gender = reader["gender"].ToString()[0]
                                    };

                                    // Return the registered user details along with a message
                                    return Ok(new { Message = "User account created", User = registeredUser });
                                }
                                else
                                {
                                    // Failed to fetch registered user details
                                    return InternalServerError(new Exception("Failed to fetch registered user details"));
                                }
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
