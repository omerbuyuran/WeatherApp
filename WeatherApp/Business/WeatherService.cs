using Microsoft.Extensions.Logging;
using Models.Request;
using WeatherAPIClient.Service;
using WeatherApp.Interfaces;
using WeatherApp.Models.Model;
using WeatherApp.Models.Responses;
using WeatherStackClient.Service;

namespace WeatherApp.Business
{
    public class WeatherService : IWeatherService
    {
        private readonly IWeatherRepository weatherRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IAPIService _apiService;
        private readonly IStackService _stackService;
        private readonly Dictionary<string, TaskCompletionSource<WeatherResponse>> _requestsInProgress;
        private readonly TimeSpan _requestWaitTime = TimeSpan.FromSeconds(5);
        private readonly ILogger<WeatherService> _logger;
        public WeatherService(IWeatherRepository weatherRepository, IUnitOfWork unitOfWork, IAPIService apiService, IStackService stackService, ILogger<WeatherService> logger)
        {
            this.weatherRepository = weatherRepository;
            this.unitOfWork = unitOfWork;
            _apiService = apiService;
            _stackService = stackService;
            _requestsInProgress = new Dictionary<string, TaskCompletionSource<WeatherResponse>>();
            _logger = logger;
        }

        public async Task<WeatherResponse> AddWeather(WeatherInfo weatherInfo)
        {
            try
            {
                _logger.LogInformation("Starting AddWeather for {City} on {Date}", weatherInfo.City, weatherInfo.Date);
                await weatherRepository.AddWeatherAsync(weatherInfo);
                await unitOfWork.CompleteAsync();
                _logger.LogInformation("Weather info added successfully for {City} on {Date}", weatherInfo.City, weatherInfo.Date);
                return new WeatherResponse(weatherInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding weather info for {City}", weatherInfo.City);
                return new WeatherResponse(ex.Message);
            }
        }

        public async Task<WeatherResponse> GetAverageWeatherFromApisAsync(WeatherAPIRequest request)
        {
            try
            {
                _logger.LogInformation("Starting GetAverageWeatherFromApisAsync for {City}", request.q);
                TaskCompletionSource<WeatherResponse> tcs;
                lock (_requestsInProgress)
                {
                    if (_requestsInProgress.ContainsKey(request.q.ToLowerInvariant()))
                    {
                        // Eğer daha önce aynı şehir için talep gelmişse, mevcut gruba katıl.
                        _logger.LogInformation("Existing request in progress for {City}", request.q);
                        tcs = _requestsInProgress[request.q.ToLowerInvariant()];
                    }
                    else
                    {
                        // Yeni bir talep geldiğinde 5 saniyelik bekleme başlat.
                        _logger.LogInformation("Creating new request for {City}", request.q);
                        tcs = new TaskCompletionSource<WeatherResponse>();
                        _requestsInProgress[request.q.ToLowerInvariant()] = tcs;

                        _logger.LogInformation("Initiating API request for {City}", request.q);

                        // 5 saniye bekleme ve API sorgulama işlemini başlat.
                        Task.Delay(_requestWaitTime).ContinueWith(_ => FetchWeatherData(request.q.ToLowerInvariant(), tcs));
                    }
                }

                // Kullanıcıya sonucu bekleterek döndür.
                return await tcs.Task;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching average weather for {City}", request.q);
                return new WeatherResponse(ex.Message);
            }
        }

        private async Task FetchWeatherData(string city, TaskCompletionSource<WeatherResponse> tcs)
        {
            _logger.LogInformation("Starting FetchWeatherData for {City}", city);
            try
            {
                WeatherInfo weatherInfo = await weatherRepository.GetWeatherByDateAsync(DateTime.Now);
                if (weatherInfo == null)
                {
                    _logger.LogInformation("No existing weather data found in database for {City}. Fetching from APIs.", city);
                    decimal weatherApiTemp = 0;
                    decimal stackApiTemp = 0;

                    WeatherAPIRequest req = new WeatherAPIRequest
                    {
                        q = city,
                        days = 1,
                    };

                    var apiServiceResult = await _apiService.GetWeather(req);

                    if (apiServiceResult.Error != null)
                    {
                        _logger.LogWarning("API Service returned an error for {City}: {Error}", city, apiServiceResult.Error);
                    }
                    else
                    {
                        weatherApiTemp = Convert.ToDecimal(apiServiceResult.Current.TemperatureCelsius);
                    }

                    WeatherStackRequest stackReq = new WeatherStackRequest
                    {
                        query = city,
                    };
                    var stackServiceResult = await _stackService.GetWeather(stackReq);


                    if (stackServiceResult.ApiResponse != null)
                    {
                        _logger.LogWarning("Stack Service returned an error for {City}");
                    }
                    else
                    {
                        stackApiTemp = stackServiceResult.Current.Temperature;
                    }

                    if (stackServiceResult.ApiResponse != null && apiServiceResult.Error != null)
                    {
                        _logger.LogError("Both API services failed to provide data for {City}", city);
                        tcs.TrySetResult(new WeatherResponse("Hava durumu bilgisi bulunamadı"));
                        return;
                    }                    

                    // Ortalama sıcaklık hesaplanır.
                    decimal averageTemp = 0;
                    if (weatherApiTemp > 0 && stackApiTemp > 0)
                    {
                        averageTemp = (weatherApiTemp + stackApiTemp) / 2;
                    }
                    else if (weatherApiTemp > 0)
                    {
                        averageTemp = weatherApiTemp;
                    }
                    else
                    {
                        averageTemp = stackApiTemp;
                    }

                    WeatherInfo info = new WeatherInfo
                    {
                        City = city,
                        Date = DateTime.Now,
                        Temperature = averageTemp
                    };

                    // Yeni hava durumu kaydı veritabanına eklenir.
                    await weatherRepository.AddWeatherAsync(info);
                    await unitOfWork.CompleteAsync();
                    _logger.LogInformation("Weather data for {City} saved successfully with average temperature {Temperature}", city, averageTemp);

                    tcs.TrySetResult(new WeatherResponse(info));
                }
                else
                {
                    tcs.TrySetResult(new WeatherResponse(weatherInfo));
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during FetchWeatherData for {City}", city);
                tcs.TrySetResult(new WeatherResponse(ex.Message));
            }
            finally
            {
                lock (_requestsInProgress)
                {
                    _requestsInProgress.Remove(city);
                    _logger.LogInformation("Request for {City} removed from in-progress list", city);
                }
            }
        }
        public async Task<WeatherResponse> GetWeatherByIdAsync(int weatherInfoId)
        {
            try
            {
                WeatherInfo weatherInfo = await weatherRepository.GetWeatherByIdAsync(weatherInfoId);
                if (weatherInfo == null)
                {
                    _logger.LogWarning("Weather info not found for ID {WeatherInfoId}", weatherInfoId);
                    return new WeatherResponse("Hava durumu bilgisi bulunamadı");
                }
                else
                {
                    _logger.LogInformation("Weather info found for ID {WeatherInfoId}", weatherInfoId);
                    return new WeatherResponse(weatherInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving weather info for ID {WeatherInfoId}", weatherInfoId);
                return new WeatherResponse(ex.Message);
            }
        }

        public async Task<WeatherInfoListResponse> ListAsync()
        {
            try
            {
                IEnumerable<WeatherInfo> weatherInfos = await weatherRepository.ListAsync();
                return new WeatherInfoListResponse(weatherInfos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing weather infos");
                return new WeatherInfoListResponse(ex.Message);
            }
        }

        public async Task<WeatherResponse> RemoveWeather(int weatherInfoId)
        {
            try
            {
                WeatherInfo weatherInfo = await weatherRepository.GetWeatherByIdAsync(weatherInfoId);
                if (weatherInfo == null)
                {
                    _logger.LogWarning("Weather info not found for ID {WeatherInfoId}", weatherInfoId);
                    return new WeatherResponse("Hava durumu bilgisi bulunamadı");
                }
                else
                {
                    _logger.LogInformation("Weather info found for ID {WeatherInfoId}, removing it", weatherInfoId);
                    await weatherRepository.RemoveWeatherAsync(weatherInfoId);
                    await unitOfWork.CompleteAsync();
                    _logger.LogInformation("Successfully removed weather info for ID {WeatherInfoId}", weatherInfoId);
                    return new WeatherResponse(weatherInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing weather info for ID {WeatherInfoId}", weatherInfoId);
                return new WeatherResponse(ex.Message);
            }
        }

        public async Task<WeatherResponse> UpdateWeather(WeatherInfo weatherInfo, int weatherInfoId)
        {
            try
            {
                var firstWeather = await weatherRepository.GetWeatherByIdAsync(weatherInfoId);
                if (firstWeather == null)
                {
                    _logger.LogWarning("Weather info not found for ID {WeatherInfoId}", weatherInfoId);
                    return new WeatherResponse("Hava durumu bilgisi bulunamadı");
                }
                else
                {
                    _logger.LogInformation("Weather info found for ID {WeatherInfoId}, updating it", weatherInfoId);
                    firstWeather.City = weatherInfo.City;
                    firstWeather.Date = weatherInfo.Date;
                    firstWeather.Temperature = weatherInfo.Temperature;
                    weatherRepository.UpdateWeather(firstWeather);
                    await unitOfWork.CompleteAsync();
                    _logger.LogInformation("Successfully updated weather info for ID {WeatherInfoId}", weatherInfoId);

                    return new WeatherResponse(firstWeather);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating weather info for ID {WeatherInfoId}", weatherInfoId);
                return new WeatherResponse(ex.Message);
            }
        }
    }
}









//public async Task<WeatherResponse> GetAverageWeatherFromApisAsync(WeatherAPIRequest request)
//{
//    try
//    {
//        WeatherInfo weatherInfo = await weatherRepository.GetWeatherByDateAsync(DateTime.Now);
//        if (weatherInfo == null)
//        {
//            decimal weatherApiTemp = 0;
//            decimal stackApiTemp = 0;
//            WeatherAPIRequest req = new WeatherAPIRequest
//            {
//                q = request.q.ToLowerInvariant(),
//                days = 1,
//            };
//            var apiServiceResult = await _apiService.GetWeather(req);

//            if(apiServiceResult.Error != null)
//            {
//                //logla
//            }
//            else
//            {
//                weatherApiTemp = Convert.ToDecimal(apiServiceResult.Current.TemperatureCelsius);
//            }

//            WeatherStackRequest stackReq = new WeatherStackRequest
//            {
//                query = request.q.ToLowerInvariant(),
//            };
//            var stackServiceResult = await _stackService.GetWeather(stackReq);

//            if(stackServiceResult.ApiResponse != null)
//            {
//                //logla
//            }
//            else
//            {
//                stackApiTemp = stackServiceResult.Current.Temperature;
//            }

//            if(stackServiceResult.ApiResponse != null && apiServiceResult.Error != null)
//            {
//                return new WeatherResponse("Hava durumu bilgisi bulunamadı");
//            }

//            if(weatherApiTemp > 0 && stackApiTemp > 0)
//            {
//                var averageTemp = (weatherApiTemp + stackApiTemp)/2;
//                string city = request.q.ToLowerInvariant();
//                WeatherInfo info = new WeatherInfo
//                {
//                    City = city,
//                    Date = DateTime.Now,
//                    Temperature = averageTemp
//                };

//                await AddWeather(info);

//                return new WeatherResponse(info);
//            }
//            else if(weatherApiTemp > 0)
//            {
//                string city = request.q.ToLowerInvariant();
//                WeatherInfo info = new WeatherInfo
//                {
//                    City = city,
//                    Date = DateTime.Now,
//                    Temperature = weatherApiTemp
//                };

//                await AddWeather(info);

//                return new WeatherResponse(info);
//            }
//            else if(stackApiTemp> 0)
//            {
//                string city = request.q.ToLowerInvariant();
//                WeatherInfo info = new WeatherInfo
//                {
//                    City = city,
//                    Date = DateTime.Now,
//                    Temperature = stackApiTemp
//                };

//                await AddWeather(info);

//                return new WeatherResponse(info);
//            }
//            return new WeatherResponse("Hava durumu bilgisi bulunamadı");
//        }
//        else
//        {
//            return new WeatherResponse(weatherInfo);
//        }
//    }
//    catch (Exception ex)
//    {

//        return new WeatherResponse(ex.Message);
//    }

//}