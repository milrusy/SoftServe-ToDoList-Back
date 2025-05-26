using TodoList.WebAPI.Models;

namespace TodoList.WebAPI.Services.Interfaces
{
    public interface ITaskItemDatabaseService
    {
        Task<TaskItem> GetByIdAsync(int userId, int taskItemId);
        Task<IEnumerable<TaskItem>> GetAllByTodoListIdAsync(int userId, int todoListId);
        Task<TaskItem> CreateAsync(int userId, int todoListId, TaskItem taskItem);
        Task<TaskItem> UpdateAsync(int userId, int taskItemId, TaskItem taskItem);
        Task<bool> DeleteAsync(int userId, int taskItemId);
        Task<bool> ChangeTaskStatusAsync(int userId, int taskId, string status);
    }

}
