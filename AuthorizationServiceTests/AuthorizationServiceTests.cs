using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using TodoList.WebAPI.Models;

namespace TodoList.WebAPI.Services.Tests
{
    public class AuthorizationServiceTests
    {
        private readonly Mock<IHttpContextAccessor> httpContextAccessorMock;
        private readonly AuthorizationService authorizationService;

        public AuthorizationServiceTests()
        {
            this.httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            this.authorizationService = new AuthorizationService(httpContextAccessorMock.Object);
        }

        [Fact]
        public void CreateToken_ReturnsValidJwtSecurityToken()
        {
            // Arrange
            var user = new User
            {
                Id = 42,
                Username = "testuser"
            };

            // Act
            var token = authorizationService.CreateToken(user);

            // Assert
            Assert.NotNull(token);
            Assert.Equal("todolist.webapi", token.Issuer);
            Assert.Equal("todolist.webapi", token.Audiences.First());
            Assert.Contains(token.Claims, c => c.Type == ClaimTypes.NameIdentifier && c.Value == "42");
            Assert.Contains(token.Claims, c => c.Type == ClaimTypes.Name && c.Value == "testuser");
        }

        [Fact]
        public void GetUserId_ReturnsUserId_WhenClaimExists()
        {
            // Arrange
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "123"),
            new Claim(ClaimTypes.Name, "user")
        };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            var context = new DefaultHttpContext
            {
                User = principal
            };

            httpContextAccessorMock.Setup(h => h.HttpContext).Returns(context);

            // Act
            var userId = authorizationService.GetUserId();

            // Assert
            Assert.Equal(123, userId);
        }

        [Fact]
        public void GetUserId_ThrowsException_WhenUserIsNotAuthenticated()
        {
            // Arrange
            httpContextAccessorMock.Setup(h => h.HttpContext).Returns((HttpContext)null);

            // Act & Assert
            var ex = Assert.Throws<UnauthorizedAccessException>(() => authorizationService.GetUserId());
            Assert.Equal("User is not authenticated.", ex.Message);
        }

        [Fact]
        public void GetUserId_ThrowsException_WhenUserIdClaimIsMissing()
        {
            // Arrange
            var identity = new ClaimsIdentity(new List<Claim>(), "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            var context = new DefaultHttpContext
            {
                User = principal
            };

            httpContextAccessorMock.Setup(h => h.HttpContext).Returns(context);

            // Act & Assert
            var ex = Assert.Throws<UnauthorizedAccessException>(() => authorizationService.GetUserId());
            Assert.Equal("User ID claim not found or invalid.", ex.Message);
        }
    }
}
