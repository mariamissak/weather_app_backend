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
                    using (var cmdUpdate = new SqlCommand("UPDATE Registration SET name = @Name, age = @Age, gender = @Gender, " +
                                                          "temperature_thres = @TemperatureThres, humidity_thres = @HumidityThres, " +
                                                          "pm25_thres = @PM25Thres, pm10_thres = @PM10Thres, " +
                                                          "co_thres = @COThres, pressure_mb_thres = @PressureMBThres, " +
                                                          "visibility_km_thres = @VisibilityKMThres, wind_kph_thres = @WindKPHThres, " +
                                                          "uv_thres = @UVThres WHERE id = @UserId", conn))
                    {
                        cmdUpdate.Parameters.AddWithValue("@Name", userDetails.name);
                        cmdUpdate.Parameters.AddWithValue("@Age", userDetails.age);
                        cmdUpdate.Parameters.AddWithValue("@Gender", userDetails.gender);
                        cmdUpdate.Parameters.AddWithValue("@TemperatureThres", userDetails.thresholds?.temperature_thres ?? 0);
                        cmdUpdate.Parameters.AddWithValue("@HumidityThres", userDetails.thresholds?.humidity_thres ?? 0);
                        cmdUpdate.Parameters.AddWithValue("@PM25Thres", userDetails.thresholds?.pm25_thres ?? 0);
                        cmdUpdate.Parameters.AddWithValue("@PM10Thres", userDetails.thresholds?.pm10_thres ?? 0);
                        cmdUpdate.Parameters.AddWithValue("@COThres", userDetails.thresholds?.co_thres ?? 0);
                        cmdUpdate.Parameters.AddWithValue("@PressureMBThres", userDetails.thresholds?.pressure_mb_thres ?? 0);
                        cmdUpdate.Parameters.AddWithValue("@VisibilityKMThres", userDetails.thresholds?.visibility_km_thres ?? 0);
                        cmdUpdate.Parameters.AddWithValue("@WindKPHThres", userDetails.thresholds?.wind_kph_thres ?? 0);
                        cmdUpdate.Parameters.AddWithValue("@UVThres", userDetails.thresholds?.uv_thres ?? 0);
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
