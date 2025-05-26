using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using TodoList.WebAPI.Controllers;
using TodoList.WebAPI.Models;
using TodoList.WebAPI.Models.DTOs;
using TodoList.WebAPI.Services.Interfaces;
using Xunit;

namespace TodoList.WebAPI.Tests
{
    public class LoginControllerTests
    {
        private readonly Mock<IUserService> userServiceMock = new();
        private readonly Mock<IAuthorizationService> authServiceMock = new();
        private readonly LoginController controller;

        public LoginControllerTests()
        {
            this.controller = new LoginController(userServiceMock.Object, authServiceMock.Object);
        }

        [Fact]
        public async Task Register_ReturnsOk_WhenRegistrationSuccessful()
        {
            // Arrange
            var registerDto = new RegisterDto("test", "test@example.com", "pass");
            var user = new User { Id = 1, Username = "test", Email = "test@example.com" };

            this.userServiceMock.Setup(s => s.RegisterAsync(registerDto.Username, registerDto.Email, registerDto.Password))
                           .ReturnsAsync(user);

            var fakeToken = new JwtSecurityToken();
            this.authServiceMock.Setup(s => s.CreateToken(user)).Returns(fakeToken);

            // Act
            var result = await this.controller.Register(registerDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<AuthResponseDto>(okResult.Value);
            Assert.False(string.IsNullOrEmpty(response.Token));
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenRegistrationFails()
        {
            // Arrange
            var registerDto = new RegisterDto("exists", "email", "1234");
            this.userServiceMock.Setup(s => s.RegisterAsync(registerDto.Username, registerDto.Email, registerDto.Password))
                        .ReturnsAsync((User)null);

            // Act
            var result = await this.controller.Register(registerDto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("User registration failed. Username may already exist.", badRequest.Value);
        }

        [Fact]
        public async Task Login_ReturnsOk_WhenAuthenticationSuccessful()
        {
            // Arrange
            var loginDto = new LoginDto("test", "pass");
            var user = new User { Id = 1, Username = "test", Email = "test@example.com" };

            this.userServiceMock.Setup(s => s.AuthenticateAsync(loginDto.Username, loginDto.Password))
                           .ReturnsAsync(user);

            var fakeToken = new JwtSecurityToken();
            this.authServiceMock.Setup(s => s.CreateToken(user)).Returns(fakeToken);

            // Act
            var result = await this.controller.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<AuthResponseDto>(okResult.Value);
            Assert.False(string.IsNullOrEmpty(response.Token));
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenAuthenticationFails()
        {
            // Arrange
            var loginDto = new LoginDto("wrong", "wrong");

            this.userServiceMock.Setup(s => s.AuthenticateAsync(loginDto.Username, loginDto.Password))
                           .ReturnsAsync((User)null);

            // Act
            var result = await this.controller.Login(loginDto);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }
    }
}
