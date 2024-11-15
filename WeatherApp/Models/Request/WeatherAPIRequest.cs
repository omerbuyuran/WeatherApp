using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Request
{
    public class WeatherAPIRequest
    {
        [Required]
        public string q { get; set; }
        [Required]
        public int days { get; set; } = 1;
    }
}
