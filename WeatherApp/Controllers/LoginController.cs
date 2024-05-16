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

                                // Check if threshold values are available
                                if (reader["temperature_thres"] != DBNull.Value)
                                {
                                    user.thresholds = new Thresholds
                                    {
                                        temperature_thres = Convert.ToSingle(reader["temperature_thres"]),
                                        humidity_thres = Convert.ToSingle(reader["humidity_thres"]),
                                        pm25_thres = Convert.ToSingle(reader["pm25_thres"]),
                                        pm10_thres = Convert.ToSingle(reader["pm10_thres"]),
                                        co_thres = Convert.ToSingle(reader["co_thres"]),
                                        pressure_mb_thres = Convert.ToSingle(reader["pressure_mb_thres"]),
                                        visibility_km_thres = Convert.ToSingle(reader["visibility_km_thres"]),
                                        wind_kph_thres = Convert.ToSingle(reader["wind_kph_thres"]),
                                        uv_thres = Convert.ToInt32(reader["uv_thres"])
                                    };
                                }
                                else
                                {
                                    // If threshold values are not available, set them to zeros
                                    user.thresholds = new Thresholds
                                    {
                                        temperature_thres = 0,
                                        humidity_thres = 0,
                                        pm25_thres = 0,
                                        pm10_thres = 0,
                                        co_thres = 0,
                                        pressure_mb_thres = 0,
                                        visibility_km_thres = 0,
                                        wind_kph_thres = 0,
                                        uv_thres = 0
                                    };
                                }

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
