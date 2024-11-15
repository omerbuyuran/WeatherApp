

using Models.Request;
using WeatherAPIClient.Service;
using WeatherApp.Interfaces;
using WeatherApp.Models.Model;
using WeatherApp.Models.Responses;

namespace WeatherApp.Business
{
    public class LoginService : ILoginService
    {
        private readonly IUserRepository userRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IAPIService _apiService;
        public LoginService(IUserRepository userRepository, IUnitOfWork unitOfWork, IAPIService apiService)
        {
            this.userRepository = userRepository;
            this.unitOfWork = unitOfWork;
            _apiService = apiService;
        }

        public void Login(User user)
        {
            throw new NotImplementedException();
        }

        public void Logout(User user, int userId)
        {
            throw new NotImplementedException();
        }
    }
}
