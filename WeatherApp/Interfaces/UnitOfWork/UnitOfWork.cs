using WeatherApp.Interfaces;
using WeatherApp.Entities;

namespace WeatherApp.Models.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly WeatherAppDBContext context;
        public UnitOfWork(WeatherAppDBContext context)
        {
            this.context = context;
        }
        public async Task CompleteAsync()
        {
            await this.context.SaveChangesAsync();
        }
    }
}
