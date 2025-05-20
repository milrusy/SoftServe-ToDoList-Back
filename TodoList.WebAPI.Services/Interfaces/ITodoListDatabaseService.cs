using TodoList.WebAPI.Models;

namespace TodoList.WebAPI.Services.Interfaces
{
    public interface ITodoListDatabaseService
    {
        Task<Models.TodoList> GetByIdAsync(int id);
        Task<IEnumerable<Models.TodoList>> GetAllAsync();
        Task<Models.TodoList> CreateAsync(Models.TodoList todoList);
        Task<Models.TodoList> UpdateAsync(int id, Models.TodoList todoList);
        Task<bool> DeleteAsync(int id);
    }
}
