using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using TodoList.WebAPI.Models.DTOs;
using TodoList.WebAPI.Services.Interfaces;

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
            var user = await userService.RegisterAsync(dto.Username, dto.Email, dto.Password);
            if (user == null)
            {
                return BadRequest("User registration failed. Username may already exist.");
            }

            return Ok(new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(authorizationService.CreateToken(user))
            });
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            var user = await userService.AuthenticateAsync(request.Username, request.Password);
            if (user != null)
            {
                return Ok(new AuthResponseDto
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(authorizationService.CreateToken(user))
                });
            }
            return Unauthorized();
        }
    }
}
