using System.ComponentModel.DataAnnotations;

namespace WeatherApp.Models.Request
{
    public class UserRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string? Type { get; set; }
    }
}
