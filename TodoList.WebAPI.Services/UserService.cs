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
        private readonly TodoListDbContext context;
        private readonly IPasswordHasher<UserEntity> passwordHasher;

        public UserService(TodoListDbContext context)
        {
            this.context = context;
            this.passwordHasher = new PasswordHasher<UserEntity>();
        }

        public async Task<User?> RegisterAsync(string username, string email, string password)
        {
            if (await this.context.Users.AnyAsync(u => u.Username == username))
            {
                return null;
            }

            if (await this.context.Users.AnyAsync(u => u.Email == email))
            {
                return null;
            }

            var user = new UserEntity { Username = username, Email = email };
            user.PasswordHash = this.passwordHasher.HashPassword(user, password);

            this.context.Users.Add(user);
            await context.SaveChangesAsync();
            return UserMapper.ToModel(user);
        }

        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            var user = await this.context.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return null;
            }

            var result = this.passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            return result == PasswordVerificationResult.Success ? UserMapper.ToModel(user) : null;
        }
    }
}
