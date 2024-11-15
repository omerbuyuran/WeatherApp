using Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherApp.Models.Model;

namespace Models.Model
{
    public class MyAccountViewModel
    {
        public User User { get; set; }
        public List<Favorite> Favorite { get; set; }
        public WeatherAPIRequest Request { get; set; }
    }
}
