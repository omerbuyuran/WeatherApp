using WeatherApp.Models.Model;
using WeatherApp.Models.Responses;

namespace WeatherApp.Interfaces
{
    public interface IUserService
    {
        Task<UserListResponse> ListAsync();
        Task<UserResponse> AddUser(User user);
        Task<UserResponse> UpdateUser(User user, int userId);
        Task<UserResponse> RemoveUser(int userId);
        Task<UserResponse> GetUserByIdAsync(int userId);
    }
}
