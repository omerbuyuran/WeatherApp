using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherStackClient.Model
{
    public class StackConfig
    {
        public string ServiceUrl { get; set; }
        public string APIKey { get; set; }
        public int TimeoutSeconds { get; set; }
    }
}
