using Models.Request;
using Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherStackClient.Model;

namespace WeatherStackClient.Service
{
    public interface IStackService
    {
        Task<WeatherStackResponse> GetWeather(WeatherStackRequest request);
    }
}
