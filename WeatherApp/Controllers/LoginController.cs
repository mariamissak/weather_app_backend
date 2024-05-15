using System;
using System.Collections.Generic;
using System.Configuration;
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
    public class LoginController : ApiController
    {
        private readonly string _connectionString = "Data Source=DESKTOP-IRBSTH0\\SQLEXPRESS01;Initial Catalog=WeatherApp;Integrated Security=True;Connect Timeout=30;Encrypt=False";
        [HttpPost]
        public IHttpActionResult Login(Login login)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("SELECT * FROM Registration WHERE email = @Email AND password = @Password", conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", login.email);
                        cmd.Parameters.AddWithValue("@Password", login.password);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Extract user data from the reader
                                var user = new User
                                {
                                    id = Convert.ToInt32(reader["id"]),
                                    name = reader["name"].ToString(),
                                    email = reader["email"].ToString(),
                                    password = reader["password"].ToString(),
                                    age = Convert.ToInt32(reader["age"]),
                                    gender = Convert.ToChar(reader["gender"])
                                };

                                // Return the user object along with the "Logged in" message
                                return Ok(new { Message = "Logged in", User = user });
                            }
                            else
                            {
                                return BadRequest("Login failed");
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
