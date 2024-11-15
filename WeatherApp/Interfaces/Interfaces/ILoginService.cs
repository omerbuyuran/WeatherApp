using WeatherApp.Models.Responses;
using WeatherApp.Models.Model;

namespace WeatherApp.Interfaces
{
    public interface ILoginService
    {
        void Login(User user);
        void Logout(User user, int userId);
    }
}
