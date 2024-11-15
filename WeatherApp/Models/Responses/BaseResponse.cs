namespace WeatherApp.Models.Responses
{
    public class BaseResponse
    {
        //başarılıysa success dönecek, fail olursa message dönecek
        public bool Success { get; set; }
        public string Message { get; set; }
        public BaseResponse(bool success,string message)
        {
            this.Success = success;
            this.Message = message;
        }
    }
}
