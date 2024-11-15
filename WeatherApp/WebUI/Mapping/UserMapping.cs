using AutoMapper;
using WeatherApp.Models.Model;
using WeatherApp.Models.Request;

namespace WeatherApp.Mapping
{
    public class UserMapping:Profile
    {
        public UserMapping()
        {
            CreateMap<UserRequest, User>();
            CreateMap<User, UserRequest>();
        }
    }
}
