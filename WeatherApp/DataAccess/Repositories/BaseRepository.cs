using WeatherApp.Entities;

namespace WeatherApp.Repositories
{
    public class BaseRepository
    {
        protected readonly WeatherAppDBContext context;
        public BaseRepository(WeatherAppDBContext context)
        {
            this.context = context;
        }
    }
}
