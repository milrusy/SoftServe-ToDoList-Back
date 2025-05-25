using TodoList.WebAPI.Models;

namespace TodoList.WebAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task RegisterAsync(string username, string email, string password);
        Task<User?> AuthenticateAsync(string username, string password);
    }
}
