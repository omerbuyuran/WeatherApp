using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using WeatherApp.Business;
using WeatherApp.Interfaces;
using WeatherApp.Models.Model;
using WeatherApp.Models.Responses;
using Xunit;

namespace UnitTest
{
    public class UserServiceTest
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly UserService _userService;

        public UserServiceTest()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _userService = new UserService(_mockUserRepository.Object, _mockUnitOfWork.Object, null);
        }

        [Fact]
        public async Task AddUser_ShouldReturnUserResponse_WhenUserIsAddedSuccessfully()
        {
            // Arrange
            var user = new User { Id = 1, Name = "John", Surname = "Doe", Type = "Standard" };
            _mockUserRepository.Setup(repo => repo.AddUserAsync(user)).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(uow => uow.CompleteAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _userService.AddUser(user);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(user, result.User);
        }

        [Fact]
        public async Task AddUser_ShouldReturnErrorResponse_WhenExceptionIsThrown()
        {
            // Arrange
            var user = new User { Id = 1, Name = "John", Surname = "Doe", Type = "Standard" };
            _mockUserRepository.Setup(repo => repo.AddUserAsync(user)).Throws(new Exception("Database error"));

            // Act
            var result = await _userService.AddUser(user);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal("Database error", result.Message);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUserResponse_WhenUserIsFound()
        {
            // Arrange
            var user = new User { Id = 1, Name = "John", Surname = "Doe", Type = "Standard" };
            _mockUserRepository.Setup(repo => repo.GetUserByIdAsync(1)).ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(user, result.User);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnErrorResponse_WhenUserIsNotFound()
        {
            // Arrange
            _mockUserRepository.Setup(repo => repo.GetUserByIdAsync(1)).ReturnsAsync((User)null);

            // Act
            var result = await _userService.GetUserByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal("User bulunamadý", result.Message);
        }

        [Fact]
        public async Task ListAsync_ShouldReturnUserListResponse_WhenUsersAreFound()
        {
            // Arrange
            var users = new List<User> { new User { Id = 1, Name = "John", Surname = "Doe", Type = "Standard" } };
            _mockUserRepository.Setup(repo => repo.ListAsync()).ReturnsAsync(users);

            // Act
            var result = await _userService.ListAsync();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(users, result.UserList);
        }

        [Fact]
        public async Task ListAsync_ShouldReturnErrorResponse_WhenExceptionIsThrown()
        {
            // Arrange
            _mockUserRepository.Setup(repo => repo.ListAsync()).Throws(new Exception("Database error"));

            // Act
            var result = await _userService.ListAsync();

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal("Database error", result.Message);
        }

        [Fact]
        public async Task RemoveUser_ShouldReturnUserResponse_WhenUserIsRemovedSuccessfully()
        {
            // Arrange
            var user = new User { Id = 1, Name = "John", Surname = "Doe", Type = "Standard" };
            _mockUserRepository.Setup(repo => repo.GetUserByIdAsync(1)).ReturnsAsync(user);
            _mockUserRepository.Setup(repo => repo.RemoveUserAsync(1)).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(uow => uow.CompleteAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _userService.RemoveUser(1);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(user, result.User);
        }

        [Fact]
        public async Task RemoveUser_ShouldReturnErrorResponse_WhenUserIsNotFound()
        {
            // Arrange
            _mockUserRepository.Setup(repo => repo.GetUserByIdAsync(1)).ReturnsAsync((User)null);

            // Act
            var result = await _userService.RemoveUser(1);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal("User bulunamadý", result.Message);
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnUserResponse_WhenUserIsUpdatedSuccessfully()
        {
            // Arrange
            var existingUser = new User { Id = 1, Name = "John", Surname = "Doe", Type = "Standard" };
            var updatedUser = new User { Name = "Jane", Surname = "Smith", Type = "Admin" };
            _mockUserRepository.Setup(repo => repo.GetUserByIdAsync(1)).ReturnsAsync(existingUser);
            _mockUserRepository.Setup(repo => repo.UpdateUser(existingUser));
            _mockUnitOfWork.Setup(uow => uow.CompleteAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _userService.UpdateUser(updatedUser, 1);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(updatedUser.Name, result.User.Name);
            Assert.Equal(updatedUser.Surname, result.User.Surname);
            Assert.Equal(updatedUser.Type, result.User.Type);
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnErrorResponse_WhenUserIsNotFound()
        {
            // Arrange
            _mockUserRepository.Setup(repo => repo.GetUserByIdAsync(1)).ReturnsAsync((User)null);

            // Act
            var result = await _userService.UpdateUser(new User(), 1);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal("User bulunamadý", result.Message);
        }
    }
}