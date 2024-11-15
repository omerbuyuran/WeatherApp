using System;
using System.Collections.Generic;

namespace WeatherApp.Models.Model
{
    public partial class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Type { get; set; }
    }
}
