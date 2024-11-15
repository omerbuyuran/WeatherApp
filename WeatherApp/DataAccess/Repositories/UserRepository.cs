using Microsoft.EntityFrameworkCore;
using WeatherApp.Interfaces;
using WeatherApp.Models.Model;
using WeatherApp.Repositories;
using WeatherApp.Entities;

namespace WeatherApp.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(WeatherAppDBContext context) : base(context)
        {
        }

        public async Task AddUserAsync(User user)
        {
            await context.Users.AddAsync(user);
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await context.Users.FindAsync(userId);
        }

        public async Task<IEnumerable<User>> ListAsync()
        {
            return await context.Users.ToListAsync();
        }

        public async Task RemoveUserAsync(int userId)
        {
            var user = await GetUserByIdAsync(userId);
            context.Users.Remove(user);
        }

        public void UpdateUser(User user)
        {
            context.Users.Update(user);
        }
    }
}
