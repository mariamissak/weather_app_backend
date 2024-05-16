using System;
using System.Data.SqlClient;
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

                                // Check if threshold values are available in the database
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
