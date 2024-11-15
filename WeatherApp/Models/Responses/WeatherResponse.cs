using WeatherApp.Models.Model;

namespace WeatherApp.Models.Responses
{
    public class WeatherResponse : BaseResponse
    {
        public WeatherInfo WeatherInfo { get; set; }
        private WeatherResponse(bool success, string message, WeatherInfo weatherInfo) : base(success, message)
        {
            this.WeatherInfo = weatherInfo;
        }
        //Başarılı ise
        public WeatherResponse(WeatherInfo weatherInfo) : this(true, string.Empty, weatherInfo) { }
        //Başarısız ise
        public WeatherResponse(string message) : this(false, message, null) { }
    }
}
