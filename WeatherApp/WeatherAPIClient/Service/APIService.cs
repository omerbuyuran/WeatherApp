using Microsoft.Extensions.Options;
using Models.Request;
using Models.Responses;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WeatherAPIClient.Model;

namespace WeatherAPIClient.Service
{
    public class APIService : IAPIService
    {
        private readonly HttpClient _httpClient;

        private readonly APIConfig _config;

        public APIService(IOptions<APIConfig> options, HttpClient httpClient)
        {
            _config = options.Value;
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(_config.TimeoutSeconds);

        }


        public async Task<WeatherAPIResponse> GetWeather(WeatherAPIRequest request)
        {
            string responseContent = string.Empty;
            try
            {
                string url = String.Concat(_config.ServiceUrl, "?q=", request.q, "&days=", request.days, "&key=", _config.APIKey);
                var requestJson = JsonSerializer.Serialize(request);
                var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

                // Create HttpRequestMessage
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url)
                {
                    Content = content
                };
               
                // Send request
                var response = await _httpClient.SendAsync(httpRequestMessage);

                if (response.IsSuccessStatusCode)
                {
                    responseContent = await response.Content.ReadAsStringAsync();
                    var submitResponse = JsonSerializer.Deserialize<WeatherAPIResponse>(responseContent);
                    return submitResponse;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var error = JsonSerializer.Deserialize<WeatherAPIResponse>(errorContent);
                    return error;
                }
            }
            catch (Exception err)
            {
                throw err;

            }
        }
    }
}
