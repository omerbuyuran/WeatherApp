using WeatherApp.Models.Model;

namespace WebUI.Services
{
    public interface ILoginSession
    {
        User GetLogin();
        void SetLogin(User user);
    }
}
