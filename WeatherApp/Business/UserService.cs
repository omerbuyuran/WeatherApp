

using Models.Request;
using WeatherAPIClient.Service;
using WeatherApp.Interfaces;
using WeatherApp.Models.Model;
using WeatherApp.Models.Responses;

namespace WeatherApp.Business
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IUnitOfWork unitOfWork;
        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork, IAPIService apiService)
        {
            this.userRepository = userRepository;
            this.unitOfWork = unitOfWork;
        }

        public async Task<UserResponse> AddUser(User user)
        {
            try
            {
                await userRepository.AddUserAsync(user);
                await unitOfWork.CompleteAsync();
                return new UserResponse(user);
            }
            catch (Exception ex)
            {
                return new UserResponse(ex.Message);
            }
        }

        public async Task<UserResponse> GetUserByIdAsync(int userId)
        {
            try
            {
                User user = await userRepository.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return new UserResponse("User bulunamadı");
                }
                else
                {
                    return new UserResponse(user);
                }
            }
            catch (Exception ex)
            {
                return new UserResponse(ex.Message);
            }
        }

        public async Task<UserListResponse> ListAsync()
        {
            try
            {
                IEnumerable<User> users = await userRepository.ListAsync();
                return new UserListResponse(users);
            }
            catch (Exception ex)
            {
                return new UserListResponse(ex.Message);
            }
        }

        public async Task<UserResponse> RemoveUser(int userId)
        {
            try
            {
                User user = await userRepository.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return new UserResponse("User bulunamadı");
                }
                else
                {
                    await userRepository.RemoveUserAsync(userId);
                    await unitOfWork.CompleteAsync();
                    return new UserResponse(user);
                }
            }
            catch (Exception ex)
            {
                return new UserResponse(ex.Message);
            }
        }

        public async Task<UserResponse> UpdateUser(User user, int userId)
        {
            try
            {
                var firstUser = await userRepository.GetUserByIdAsync(userId);
                if (firstUser == null)
                {
                    return new UserResponse("User bulunamadı");
                }
                else
                {
                    firstUser.Name = user.Name;
                    firstUser.Surname = user.Surname;
                    firstUser.Type = user.Type;
                    userRepository.UpdateUser(firstUser);
                    await unitOfWork.CompleteAsync();

                    return new UserResponse(firstUser);
                }
            }
            catch (Exception ex)
            {
                return new UserResponse(ex.Message);
            }
        }
    }
}
