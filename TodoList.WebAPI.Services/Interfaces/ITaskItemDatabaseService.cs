using TodoList.WebAPI.Models;

namespace TodoList.WebAPI.Services.Interfaces
{
    public interface ITaskItemDatabaseService
    {
        Task<TaskItem> GetByIdAsync(int taskItemId);
        Task<IEnumerable<TaskItem>> GetAllByTodoListIdAsync(int todoListId);
        Task<TaskItem> CreateAsync(int todoListId, TaskItem taskItem);
        Task<TaskItem> UpdateAsync(int taskItemId, TaskItem taskItem);
        Task<bool> DeleteAsync(int taskItemId);
        Task<IEnumerable<TaskItem>> SearchAsync(string title = null, DateTime? createdAt = null, DateTime? dueDate = null);
        Task<bool> ChangeTaskStatusAsync(int userId, int taskId, string status);
    }

}
