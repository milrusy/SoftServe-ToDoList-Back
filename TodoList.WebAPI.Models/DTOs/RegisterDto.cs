using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoList.WebAPI.Models.DTOs
{
    public class RegisterDto
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public string Email { get; set; } = default!;

        public RegisterDto(string username, string email, string password)
        {
            Username = username;
            Email = email;
            Password = password;
        }
    }
}
