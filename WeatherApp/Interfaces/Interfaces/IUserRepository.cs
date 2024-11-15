using WeatherApp.Models.Model;

namespace WeatherApp.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> ListAsync();
        Task AddUserAsync(User user);
        void UpdateUser(User user);
        Task RemoveUserAsync(int userId);
        Task<User> GetUserByIdAsync(int userId);
    }
}
