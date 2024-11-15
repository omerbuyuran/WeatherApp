using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models.Responses
{
    public class WeatherAPIErrorResponse
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }
        
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
