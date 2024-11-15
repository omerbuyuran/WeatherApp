using WeatherApp.Models.Model;

namespace WeatherApp.Models.Responses
{
    public class UserListResponse: BaseResponse
    {
        public IEnumerable<User> UserList { get; set; }
        private UserListResponse(bool success, string message, IEnumerable<User> userList) : base(success, message)
        {
            this.UserList = userList;
        }
        //Başarılı ise
        public UserListResponse(IEnumerable<User> userList) : this(true, string.Empty, userList) { }
        //Başarısız ise
        public UserListResponse(string message) : this(false, message, null) { }
    }
}
