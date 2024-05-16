using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeatherApp.Models
{
    public class Thresholds
    {
        public float temperature_thres { get; set; }
        public float humidity_thres { get; set; }
        public float pm25_thres { get; set; }
        public float pm10_thres { get; set; }
        public float co_thres { get; set; }
        public float pressure_mb_thres { get; set; }
        public float visibility_km_thres { get; set; }
        public float wind_kph_thres { get; set; }
        public int uv_thres { get; set; }


    }
}