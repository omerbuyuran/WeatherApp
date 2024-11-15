using WeatherApp.Models.Model;

namespace WeatherApp.Models.Responses
{
    public class UserResponse : BaseResponse
    {
        public User User { get; set; }
        private UserResponse(bool success, string message, User user) : base(success, message)
        {
            this.User = user;
        }
        //Başarılı ise
        public UserResponse(User user) : this(true, string.Empty, user) { }
        //Başarısız ise
        public UserResponse(string message) : this(false, message, null) { }
    }
}
