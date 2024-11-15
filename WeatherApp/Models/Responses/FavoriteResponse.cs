using WeatherApp.Models.Model;

namespace WeatherApp.Models.Responses
{
    public class FavoriteResponse : BaseResponse
    {
        public Favorite Favorite { get; set; }
        private FavoriteResponse(bool success, string message, Favorite favorite) : base(success, message)
        {
            this.Favorite = favorite;
        }
        //Başarılı ise
        public FavoriteResponse(Favorite favorite) : this(true, string.Empty, favorite) { }
        //Başarısız ise
        public FavoriteResponse(string message):this(false, message, null) { }
    }
}
