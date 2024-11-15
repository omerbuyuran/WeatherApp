using Models.Request;
using Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherAPIClient.Model;

namespace WeatherAPIClient.Service
{
    public interface IAPIService
    {
        Task<WeatherAPIResponse> GetWeather(WeatherAPIRequest request);
    }
}
