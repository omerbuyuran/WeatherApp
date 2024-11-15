using WeatherApp.Models.Model;

namespace WeatherApp.Interfaces
{
    public interface IWeatherRepository
    {
        Task<IEnumerable<WeatherInfo>> ListAsync();
        Task AddWeatherAsync(WeatherInfo weatherInfo);
        void UpdateWeather(WeatherInfo weatherInfo);
        Task RemoveWeatherAsync(int weatherInfoId);
        Task<WeatherInfo> GetWeatherByIdAsync(int weatherInfoId);
        Task<WeatherInfo> GetWeatherByDateAsync(DateTime weatherInfoDate);
    }
}
