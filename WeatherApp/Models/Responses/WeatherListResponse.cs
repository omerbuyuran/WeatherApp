using WeatherApp.Models.Model;

namespace WeatherApp.Models.Responses
{
    public class WeatherInfoListResponse : BaseResponse
    {
        public IEnumerable<WeatherInfo> WeatherInfoList { get; set; }
        private WeatherInfoListResponse(bool success, string message, IEnumerable<WeatherInfo> weatherInfoList) : base(success, message)
        {
            this.WeatherInfoList = weatherInfoList;
        }
        //Başarılı ise
        public WeatherInfoListResponse(IEnumerable<WeatherInfo> weatherInfoList) : this(true, string.Empty, weatherInfoList) { }
        //Başarısız ise
        public WeatherInfoListResponse(string message) : this(false, message, null) { }
    }
}
