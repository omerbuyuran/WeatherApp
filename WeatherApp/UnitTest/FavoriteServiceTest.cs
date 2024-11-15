using Business;
using Serilog;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherApp.Interfaces;
using WeatherApp.Models.Model;
using Xunit;

namespace UnitTest
{
    public class FavoriteServiceTest
    {
        private readonly Mock<IFavoriteRepository> favoriteRepositoryMock;
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
        private readonly FavoriteService favoriteService;
        private readonly Mock<ILogger> loggerMock;

        public FavoriteServiceTest()
        {
            Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/test_log.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

            favoriteRepositoryMock = new Mock<IFavoriteRepository>();
            unitOfWorkMock = new Mock<IUnitOfWork>();
            loggerMock = new Mock<ILogger>();
            favoriteService = new FavoriteService(favoriteRepositoryMock.Object, unitOfWorkMock.Object, loggerMock.Object);
        }

        [Fact]
        public async Task AddFavorite_ShouldReturnFavoriteResponse_WhenSuccessful()
        {
            // Arrange
            var favorite = new Favorite { Id = 1, CityName = "Istanbul", UserId = 123 };
            favoriteRepositoryMock.Setup(repo => repo.AddFavoriteAsync(favorite)).Returns(Task.CompletedTask);
            unitOfWorkMock.Setup(uow => uow.CompleteAsync()).Returns(Task.CompletedTask);

            // Act
            var response = await favoriteService.AddFavorite(favorite);

            // Assert
            Assert.NotNull(response);
            Assert.True(response.Success);
            Assert.Equal(favorite, response.Favorite);
        }

        [Fact]
        public async Task AddFavorite_ShouldReturnErrorResponse_WhenExceptionThrown()
        {
            // Arrange
            var favorite = new Favorite { Id = 1, CityName = "Istanbul", UserId = 123 };
            favoriteRepositoryMock.Setup(repo => repo.AddFavoriteAsync(favorite)).ThrowsAsync(new Exception("Database error"));

            // Act
            var response = await favoriteService.AddFavorite(favorite);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.Success);
            Assert.Equal("Database error", response.Message);
        }

        [Fact]
        public async Task GetFavoriteByIdAsync_ShouldReturnFavoriteResponse_WhenFavoriteExists()
        {
            // Arrange
            var favorite = new Favorite { Id = 1, CityName = "Istanbul", UserId = 123 };
            favoriteRepositoryMock.Setup(repo => repo.GetFavoriteByIdAsync(1)).ReturnsAsync(favorite);

            // Act
            var response = await favoriteService.GetFavoriteByIdAsync(1);

            // Assert
            Assert.NotNull(response);
            Assert.True(response.Success);
            Assert.Equal(favorite, response.Favorite);
        }

        [Fact]
        public async Task GetFavoriteByIdAsync_ShouldReturnErrorResponse_WhenFavoriteNotFound()
        {
            // Arrange
            favoriteRepositoryMock.Setup(repo => repo.GetFavoriteByIdAsync(1)).ReturnsAsync((Favorite)null);

            // Act
            var response = await favoriteService.GetFavoriteByIdAsync(1);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.Success);
            Assert.Equal("Favori bulunamadı", response.Message);
        }

        [Fact]
        public async Task RemoveFavorite_ShouldReturnSuccessResponse_WhenFavoriteRemoved()
        {
            // Arrange
            var favorite = new Favorite { Id = 1, CityName = "Istanbul", UserId = 123 };
            favoriteRepositoryMock.Setup(repo => repo.GetFavoriteByIdAsync(1)).ReturnsAsync(favorite);
            favoriteRepositoryMock.Setup(repo => repo.RemoveFavoriteAsync(1)).Returns(Task.CompletedTask);
            unitOfWorkMock.Setup(uow => uow.CompleteAsync()).Returns(Task.CompletedTask);

            // Act
            var response = await favoriteService.RemoveFavorite(1);

            // Assert
            Assert.NotNull(response);
            Assert.True(response.Success);
            Assert.Equal(favorite, response.Favorite);
        }

        [Fact]
        public async Task RemoveFavorite_ShouldReturnErrorResponse_WhenFavoriteNotFound()
        {
            // Arrange
            favoriteRepositoryMock.Setup(repo => repo.GetFavoriteByIdAsync(1)).ReturnsAsync((Favorite)null);

            // Act
            var response = await favoriteService.RemoveFavorite(1);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.Success);
            Assert.Equal("Favori bulunamadı", response.Message);
        }

        [Fact]
        public async Task UpdateFavorite_ShouldReturnSuccessResponse_WhenFavoriteUpdated()
        {
            // Arrange
            var existingFavorite = new Favorite { Id = 1, CityName = "Istanbul", UserId = 123 };
            var updatedFavorite = new Favorite { Id = 1, CityName = "Ankara", UserId = 456 };
            favoriteRepositoryMock.Setup(repo => repo.GetFavoriteByIdAsync(1)).ReturnsAsync(existingFavorite);
            unitOfWorkMock.Setup(uow => uow.CompleteAsync()).Returns(Task.CompletedTask);

            // Act
            var response = await favoriteService.UpdateFavorite(updatedFavorite, 1);

            // Assert
            Assert.NotNull(response);
            Assert.True(response.Success);
            Assert.Equal("Ankara", response.Favorite.CityName);
            Assert.Equal(456, response.Favorite.UserId);
        }

        [Fact]
        public async Task UpdateFavorite_ShouldReturnErrorResponse_WhenFavoriteNotFound()
        {
            // Arrange
            var updatedFavorite = new Favorite { Id = 1, CityName = "Ankara", UserId = 456 };
            favoriteRepositoryMock.Setup(repo => repo.GetFavoriteByIdAsync(1)).ReturnsAsync((Favorite)null);

            // Act
            var response = await favoriteService.UpdateFavorite(updatedFavorite, 1);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.Success);
            Assert.Equal("Favori bulunamadı", response.Message);
        }
    }
}
