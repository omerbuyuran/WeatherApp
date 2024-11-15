using WeatherApp.Models.Model;
using WebUI.ExtensionMethods;

namespace WebUI.Services
{
    public class LoginSession : ILoginSession
    {
        private IHttpContextAccessor _httpContextAccessor;

        public LoginSession(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public User GetLogin()
        {
            User user = _httpContextAccessor.HttpContext.Session.GetObject<User>("user");
            if(user == null)
            {
                _httpContextAccessor.HttpContext.Session.SetObject("user", user);
                user = _httpContextAccessor.HttpContext.Session.GetObject<User>("user");
            }
            return user;
        }

        public void SetLogin(User user)
        {
            _httpContextAccessor.HttpContext.Session.SetObject("user", user);
        }
    }
}
