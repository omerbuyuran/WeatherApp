using Microsoft.EntityFrameworkCore;
using WeatherApp.Interfaces;
using WeatherApp.Models.Model;
using WeatherApp.Repositories;
using WeatherApp.Entities;

namespace WeatherApp.Repositories
{
    public class WeatherRepository : BaseRepository, IWeatherRepository
    {
        public WeatherRepository(WeatherAppDBContext context) : base(context)
        {
        }

        public async Task AddWeatherAsync(WeatherInfo weatherInfo)
        {
            await context.WeatherInfos.AddAsync(weatherInfo);
        }

        public async Task<WeatherInfo> GetWeatherByIdAsync(int weatherInfoId)
        {
            return await context.WeatherInfos.FindAsync(weatherInfoId);
        }

        public async Task<IEnumerable<WeatherInfo>> ListAsync()
        {
            return await context.WeatherInfos.ToListAsync();
        }

        public async Task RemoveWeatherAsync(int weatherInfoId)
        {
            var weather = await GetWeatherByIdAsync(weatherInfoId);
            context.WeatherInfos.Remove(weather);
        }

        public void UpdateWeather(WeatherInfo weatherInfo)
        {
            context.WeatherInfos.Update(weatherInfo);
        }

        public async Task<WeatherInfo> GetWeatherByDateAsync(DateTime weatherInfoDate)
        {
            return await context.WeatherInfos.FirstOrDefaultAsync(x => x.Date == weatherInfoDate);
        }
    }
}
