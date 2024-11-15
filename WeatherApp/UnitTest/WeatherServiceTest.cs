using Microsoft.Extensions.Logging;
using Moq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherAPIClient.Service;
using WeatherApp.Business;
using WeatherApp.Interfaces;
using WeatherApp.Models.Model;
using WeatherStackClient.Service;
using Xunit;

namespace UnitTest
{
    public class WeatherServiceTest
    {
        private readonly Mock<IWeatherRepository> _mockWeatherRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IAPIService> _mockAPIService;
        private readonly Mock<IStackService> _mockStackService;
        private readonly WeatherService _weatherService;
        private readonly Mock<ILogger<WeatherService>> loggerMock;

        public WeatherServiceTest()
        {
            Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/test_log.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

            _mockWeatherRepository = new Mock<IWeatherRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockAPIService = new Mock<IAPIService>();
            _mockStackService = new Mock<IStackService>();
            loggerMock = new Mock<ILogger<WeatherService>>();
            _weatherService = new WeatherService(_mockWeatherRepository.Object, _mockUnitOfWork.Object, _mockAPIService.Object, _mockStackService.Object, loggerMock.Object);
        }

        [Fact]
        public async Task AddWeather_ShouldReturnSuccess_WhenWeatherInfoIsAdded()
        {
            // Arrange
            var weatherInfo = new WeatherInfo { City = "Istanbul", Date = DateTime.Now, Temperature = 25.5m };
            _mockWeatherRepository.Setup(repo => repo.AddWeatherAsync(weatherInfo)).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(uow => uow.CompleteAsync()).Returns(Task.CompletedTask);

            // Act
            var response = await _weatherService.AddWeather(weatherInfo);

            // Assert
            Assert.True(response.Success);
            Assert.Equal(weatherInfo, response.WeatherInfo);
        }

        [Fact]
        public async Task AddWeather_ShouldReturnError_WhenExceptionIsThrown()
        {
            // Arrange
            var weatherInfo = new WeatherInfo { City = "Istanbul", Date = DateTime.Now, Temperature = 25.5m };
            _mockWeatherRepository.Setup(repo => repo.AddWeatherAsync(weatherInfo)).Throws(new Exception("Database error"));

            // Act
            var response = await _weatherService.AddWeather(weatherInfo);

            // Assert
            Assert.False(response.Success);
            Assert.Equal("Database error", response.Message);
        }

        [Fact]
        public async Task GetWeatherByIdAsync_ShouldReturnWeatherInfo_WhenWeatherInfoExists()
        {
            // Arrange
            var weatherInfo = new WeatherInfo { Id = 1, City = "Istanbul", Date = DateTime.Now, Temperature = 20m };
            _mockWeatherRepository.Setup(repo => repo.GetWeatherByIdAsync(1)).ReturnsAsync(weatherInfo);

            // Act
            var response = await _weatherService.GetWeatherByIdAsync(1);

            // Assert
            Assert.True(response.Success);
            Assert.Equal(weatherInfo, response.WeatherInfo);
        }

        [Fact]
        public async Task GetWeatherByIdAsync_ShouldReturnError_WhenWeatherInfoDoesNotExist()
        {
            // Arrange
            _mockWeatherRepository.Setup(repo => repo.GetWeatherByIdAsync(1)).ReturnsAsync((WeatherInfo)null);

            // Act
            var response = await _weatherService.GetWeatherByIdAsync(1);

            // Assert
            Assert.False(response.Success);
            Assert.Equal("Hava durumu bilgisi bulunamadı", response.Message);
        }

        [Fact]
        public async Task RemoveWeather_ShouldReturnSuccess_WhenWeatherInfoIsRemoved()
        {
            // Arrange
            var weatherInfo = new WeatherInfo { Id = 1, City = "Istanbul", Date = DateTime.Now, Temperature = 20m };
            _mockWeatherRepository.Setup(repo => repo.GetWeatherByIdAsync(1)).ReturnsAsync(weatherInfo);
            _mockWeatherRepository.Setup(repo => repo.RemoveWeatherAsync(1)).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(uow => uow.CompleteAsync()).Returns(Task.CompletedTask);

            // Act
            var response = await _weatherService.RemoveWeather(1);

            // Assert
            Assert.True(response.Success);
            Assert.Equal(weatherInfo, response.WeatherInfo);
        }

        [Fact]
        public async Task RemoveWeather_ShouldReturnError_WhenWeatherInfoDoesNotExist()
        {
            // Arrange
            _mockWeatherRepository.Setup(repo => repo.GetWeatherByIdAsync(1)).ReturnsAsync((WeatherInfo)null);

            // Act
            var response = await _weatherService.RemoveWeather(1);

            // Assert
            Assert.False(response.Success);
            Assert.Equal("Hava durumu bilgisi bulunamadı", response.Message);
        }

        [Fact]
        public async Task UpdateWeather_ShouldReturnSuccess_WhenWeatherInfoIsUpdated()
        {
            // Arrange
            var existingWeatherInfo = new WeatherInfo { Id = 1, City = "Ankara", Date = DateTime.Now, Temperature = 15m };
            var updatedWeatherInfo = new WeatherInfo { City = "Istanbul", Date = DateTime.Now, Temperature = 25m };

            _mockWeatherRepository.Setup(repo => repo.GetWeatherByIdAsync(1)).ReturnsAsync(existingWeatherInfo);
            _mockUnitOfWork.Setup(uow => uow.CompleteAsync()).Returns(Task.CompletedTask);

            // Act
            var response = await _weatherService.UpdateWeather(updatedWeatherInfo, 1);

            // Assert
            Assert.True(response.Success);
            Assert.Equal("Istanbul", response.WeatherInfo.City);
            Assert.Equal(25m, response.WeatherInfo.Temperature);
        }

        [Fact]
        public async Task UpdateWeather_ShouldReturnError_WhenWeatherInfoDoesNotExist()
        {
            // Arrange
            var weatherInfo = new WeatherInfo { City = "Istanbul", Date = DateTime.Now, Temperature = 25m };
            _mockWeatherRepository.Setup(repo => repo.GetWeatherByIdAsync(1)).ReturnsAsync((WeatherInfo)null);

            // Act
            var response = await _weatherService.UpdateWeather(weatherInfo, 1);

            // Assert
            Assert.False(response.Success);
            Assert.Equal("Hava durumu bilgisi bulunamadı", response.Message);
        }
    }
}
