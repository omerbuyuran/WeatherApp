using WeatherApp.Models.Model;

namespace WeatherApp.Models.Responses
{
    public class FavoriteListResponse:BaseResponse
    {
        public IEnumerable<Favorite> FavoriteList { get; set; }
        private FavoriteListResponse(bool success, string message,IEnumerable<Favorite> favoriteList) : base(success, message)
        {
            this.FavoriteList = favoriteList;
        }
        //Başarılı ise
        public FavoriteListResponse(IEnumerable<Favorite> favoriteList) : this(true, string.Empty, favoriteList) { }
        //Başarısız ise
        public FavoriteListResponse(string message) : this(false, message, null) { }

    }
}
