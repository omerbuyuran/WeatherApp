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
using WeatherStackClient.Model;

namespace WeatherStackClient.Service
{
    public class StackService : IStackService
    {
        private readonly HttpClient _httpClient;

        private readonly StackConfig _config;

        public StackService(IOptions<StackConfig> options, HttpClient httpClient)
        {
            _config = options.Value;
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(_config.TimeoutSeconds);

        }


        public async Task<WeatherStackResponse> GetWeather(WeatherStackRequest request)
        {
            string responseContent = string.Empty;
            try
            {
                string url = String.Concat(_config.ServiceUrl, _config.APIKey,"&query=", request.query);
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
                    var submitResponse = JsonSerializer.Deserialize<WeatherStackResponse>(responseContent);
                    return submitResponse;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var error = JsonSerializer.Deserialize<WeatherStackResponse>(errorContent);
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
