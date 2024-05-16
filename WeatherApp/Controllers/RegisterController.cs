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
                    using (var cmdInsert = new SqlCommand("INSERT INTO Registration (name, email, password, age, gender, temperature_thres, humidity_thres, pm25_thres, pm10_thres, co_thres, pressure_mb_thres, visibility_km_thres, wind_kph_thres, uv_thres) VALUES (@Name, @Email, @Password, @Age, @Gender, @TemperatureThres, @HumidityThres, @Pm25Thres, @Pm10Thres, @CoThres, @PressureMbThres, @VisibilityKmThres, @WindKphThres, @UvThres); SELECT SCOPE_IDENTITY()", conn))
                    {
                        cmdInsert.Parameters.AddWithValue("@Name", user.name);
                        cmdInsert.Parameters.AddWithValue("@Email", user.email);
                        cmdInsert.Parameters.AddWithValue("@Password", user.password);
                        cmdInsert.Parameters.AddWithValue("@Age", user.age);
                        cmdInsert.Parameters.AddWithValue("@Gender", user.gender);

                        // Add threshold values if provided
                        if (user.thresholds != null)
                        {
                            cmdInsert.Parameters.AddWithValue("@TemperatureThres", user.thresholds.temperature_thres);
                            cmdInsert.Parameters.AddWithValue("@HumidityThres", user.thresholds.humidity_thres);
                            cmdInsert.Parameters.AddWithValue("@Pm25Thres", user.thresholds.pm25_thres);
                            cmdInsert.Parameters.AddWithValue("@Pm10Thres", user.thresholds.pm10_thres);
                            cmdInsert.Parameters.AddWithValue("@CoThres", user.thresholds.co_thres);
                            cmdInsert.Parameters.AddWithValue("@PressureMbThres", user.thresholds.pressure_mb_thres);
                            cmdInsert.Parameters.AddWithValue("@VisibilityKmThres", user.thresholds.visibility_km_thres);
                            cmdInsert.Parameters.AddWithValue("@WindKphThres", user.thresholds.wind_kph_thres);
                            cmdInsert.Parameters.AddWithValue("@UvThres", user.thresholds.uv_thres);
                        }
                        else
                        {
                            // If threshold values not provided, set them to zeros
                            cmdInsert.Parameters.AddWithValue("@TemperatureThres", 0);
                            cmdInsert.Parameters.AddWithValue("@HumidityThres", 0);
                            cmdInsert.Parameters.AddWithValue("@Pm25Thres", 0);
                            cmdInsert.Parameters.AddWithValue("@Pm10Thres", 0);
                            cmdInsert.Parameters.AddWithValue("@CoThres", 0);
                            cmdInsert.Parameters.AddWithValue("@PressureMbThres", 0);
                            cmdInsert.Parameters.AddWithValue("@VisibilityKmThres", 0);
                            cmdInsert.Parameters.AddWithValue("@WindKphThres", 0);
                            cmdInsert.Parameters.AddWithValue("@UvThres", 0);
                        }

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

                                    // Add threshold values to the user object
                                    registeredUser.thresholds = new Thresholds
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
