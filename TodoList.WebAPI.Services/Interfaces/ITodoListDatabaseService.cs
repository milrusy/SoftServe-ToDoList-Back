using TodoList.WebAPI.Models;

namespace TodoList.WebAPI.Services.Interfaces
{
    public interface ITodoListDatabaseService
    {
        Task<Models.TodoList> GetByIdAsync(int userId, int id);
        Task<IEnumerable<Models.TodoList>> GetAllAsync(int userId);
        Task<Models.TodoList> CreateAsync(int userId, Models.TodoList todoList);
        Task<Models.TodoList> UpdateAsync(int userId, int id, Models.TodoList todoList);
        Task<bool> DeleteAsync(int userId, int id);
    }
}
