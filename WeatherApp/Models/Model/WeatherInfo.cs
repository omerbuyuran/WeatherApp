using System;
using System.Collections.Generic;

namespace WeatherApp.Models.Model
{
    public partial class WeatherInfo
    {
        public int Id { get; set; }
        public string? City { get; set; }
        public DateTime? Date { get; set; }
        public decimal? Temperature { get; set; }
    }
}
