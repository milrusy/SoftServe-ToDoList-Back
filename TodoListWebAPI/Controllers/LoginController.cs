using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TodoList.WebAPI.Models.DTOs;
using TodoList.WebAPI.Services;
using TodoList.WebAPI.Services.Interfaces;

namespace TodoList.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IUserService userService;

        public LoginController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            await userService.RegisterAsync(dto.Username, dto.Email, dto.Password);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            var user = await userService.AuthenticateAsync(request.Username, request.Password);
            if (user != null)
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, request.Username)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuperStrongSecretKeyAtLeast32Char!!SuperStrongSecretKeyAtLeast32Char!!SuperStrongSecretKeyAtLeast32Char!!SuperStrongSecretKeyAtLeast32Char!!SuperStrongSecretKeyAtLeast32Char!!SuperStrongSecretKeyAtLeast32Char!!SuperStrongSecretKeyAtLeast32Char!!SuperStrongSecretKeyAtLeast32Char!!"));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: "todolist.webapi",
                    audience: "todolist.webapi",
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(2),
                    signingCredentials: creds
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
            return Unauthorized();
        }
    }
}
