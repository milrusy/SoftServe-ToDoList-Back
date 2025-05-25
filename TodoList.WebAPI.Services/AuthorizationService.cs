using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TodoList.WebAPI.Models;
using TodoList.WebAPI.Services.Interfaces;

namespace TodoList.WebAPI.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public AuthorizationService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public JwtSecurityToken CreateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            Console.WriteLine("Claims:");
            foreach (var claim in claims)
            {
                Console.WriteLine($"{claim.Type} = {claim.Value}");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuperStrongSecretKeyAtLeast32Char!!SuperStrongSecretKeyAtLeast32Char!!SuperStrongSecretKeyAtLeast32Char!!SuperStrongSecretKeyAtLeast32Char!!SuperStrongSecretKeyAtLeast32Char!!SuperStrongSecretKeyAtLeast32Char!!SuperStrongSecretKeyAtLeast32Char!!SuperStrongSecretKeyAtLeast32Char!!"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(
                issuer: "todolist.webapi",
                audience: "todolist.webapi",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );
        }

        public int GetUserId()
        {
            var user = httpContextAccessor.HttpContext?.User;

            if (user == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);

            Console.WriteLine($"User ID Claim:  {userIdClaim}");

            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }

            throw new UnauthorizedAccessException("User ID claim not found or invalid.");
        }
    }
}
