using Models.Request;
using WeatherApp.Models.Model;
using WeatherApp.Models.Responses;

namespace WeatherApp.Interfaces
{
    public interface IWeatherService
    {
        Task<WeatherInfoListResponse> ListAsync();
        Task<WeatherResponse> AddWeather(WeatherInfo weatherInfo);
        Task<WeatherResponse> UpdateWeather(WeatherInfo weatherInfo, int weatherInfoId);
        Task<WeatherResponse> RemoveWeather(int weatherInfoId);
        Task<WeatherResponse> GetWeatherByIdAsync(int weatherInfoId);
        Task<WeatherResponse> GetAverageWeatherFromApisAsync(WeatherAPIRequest request);
    }
}
