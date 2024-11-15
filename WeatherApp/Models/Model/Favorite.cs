using System;
using System.Collections.Generic;

namespace WeatherApp.Models.Model
{
    public partial class Favorite
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? CityName { get; set; }
    }
}
