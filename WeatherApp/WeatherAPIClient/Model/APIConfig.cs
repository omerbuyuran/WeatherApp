using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherAPIClient.Model
{
    public class APIConfig
    {
        public string ServiceUrl { get; set; }
        public string APIKey { get; set; }
        public int TimeoutSeconds { get; set; }
    }
}
