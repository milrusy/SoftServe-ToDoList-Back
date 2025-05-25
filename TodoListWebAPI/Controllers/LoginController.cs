using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TodoList.WebAPI.Models.DTOs;
using TodoList.WebAPI.Services;
using TodoList.WebAPI.Services.Interfaces;
using TodoList.WebAPI.Services.Mappers;

namespace TodoList.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IAuthorizationService authorizationService;

        public LoginController(IUserService userService, IAuthorizationService authorizationService)
        {
            this.userService = userService;
            this.authorizationService = authorizationService;
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
            Console.WriteLine("User: " + user?.Username + " " + user?.Id);
            if (user != null)
            {
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(authorizationService.CreateToken(user))
                });
            }
            return Unauthorized();
        }
    }
}
