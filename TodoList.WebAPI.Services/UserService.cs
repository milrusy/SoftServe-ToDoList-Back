using TodoList.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TodoList.WebAPI.Services.Database.Contexts;
using TodoList.WebAPI.Services.Database.Entities;
using TodoList.WebAPI.Models;
using TodoList.WebAPI.Services.Mappers;

namespace TodoList.WebAPI.Services
{
    public class UserService : IUserService
    {
        private readonly TodoListDbContext _context;
        private readonly IPasswordHasher<UserEntity> _passwordHasher;

        public UserService(TodoListDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<UserEntity>();
        }

        public async Task RegisterAsync(string username, string email, string password)
        {
            if (await _context.Users.AnyAsync(u => u.Username == username))
                throw new InvalidOperationException("Username already exists");

            var user = new UserEntity { Username = username, Email = email };
            user.PasswordHash = _passwordHasher.HashPassword(user, password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (user == null)
                return null;

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            return result == PasswordVerificationResult.Success ? UserMapper.ToModel(user) : null;
        }
    }
}
