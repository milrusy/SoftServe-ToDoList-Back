using Microsoft.EntityFrameworkCore;
using TodoList.WebAPI.Models;
using TodoList.WebAPI.Services.Database.Contexts;
using TodoList.WebAPI.Services.Interfaces;
using TodoList.WebAPI.Services.Mappers;

namespace TodoList.WebAPI.Services
{
    public class TaskItemDatabaseService : ITaskItemDatabaseService
    {
        private readonly TodoListDbContext context;

        public TaskItemDatabaseService(TodoListDbContext context)
        {
            this.context = context;
        }

        public async Task<TaskItem> GetByIdAsync(int userId, int taskItemId)
        {
            var taskItem = await this.context.TaskItems.FirstOrDefaultAsync(t => t.Id == taskItemId);
            var todoList = await this.context.TodoLists
                .FirstOrDefaultAsync(t => t.Id == taskItem.TodoListId);
            if (todoList == null || todoList.OwnerId != userId)
            {
                throw new ArgumentException("Task item not found or does not belong to the user.", nameof(taskItemId));
            }

            if (taskItem == null)
            {
                return null;
            }

            return TaskItemMapper.ToModel(taskItem);
        }

        public async Task<IEnumerable<TaskItem>> GetAllByTodoListIdAsync(int userId, int todoListId)
        {
            var todoList = await this.context.TodoLists
                .FirstOrDefaultAsync(t => t.Id == todoListId && t.OwnerId == userId);
            if (todoList == null)
            {
                throw new ArgumentException("Todo list not found or does not belong to the user.", nameof(todoListId));
            }

            var taskItems = await this.context.TaskItems
            .Where(t => t.TodoListId == todoListId)
            .ToListAsync();

            return taskItems.Select(t => TaskItemMapper.ToModel(t));
        }

        public async Task<TaskItem> CreateAsync(int userId, int todoListId, TaskItem taskItem)
        {
            var todoList = await this.context.TodoLists
                .FirstOrDefaultAsync(t => t.Id == todoListId && t.OwnerId == userId);
            if (todoList == null)
            {
                throw new ArgumentException("Todo list not found or does not belong to the user.", nameof(todoListId));
            }
            taskItem.CreatedAt = DateTime.UtcNow;
            taskItem.TodoListId = todoListId;
            var entity = this.context.TaskItems.Add(TaskItemMapper.ToEntity(taskItem));
            _ = await this.context.SaveChangesAsync();
            return TaskItemMapper.ToModel(entity.Entity);
        }

        public async Task<TaskItem> UpdateAsync(int userId, int taskItemId, TaskItem taskItem)
        {
            var todoList = await this.context.TodoLists
                .FirstOrDefaultAsync(t => t.Id == taskItem.TodoListId && t.OwnerId == userId);
            if (todoList == null)
            {
                throw new ArgumentException("Todo list not found or does not belong to the user.");
            }
            var entity = await this.context.TaskItems.FindAsync(taskItemId);
            if (entity == null)
            {
                return null;
            }

            entity.Title = taskItem.Title;
            entity.Description = taskItem.Description;
            entity.Status = taskItem.Status.ToString();
            entity.CreatedAt = taskItem.CreatedAt;
            entity.DueDate = taskItem.DueDate;
            entity.TodoListId = taskItem.TodoListId;

            _ = await this.context.SaveChangesAsync();
            return TaskItemMapper.ToModel(entity);
        }

        public async Task<bool> DeleteAsync(int userId, int taskItemId)
        {
            var todoList = await this.context.TodoLists
                .FirstOrDefaultAsync(t => t.Id == taskItemId && t.OwnerId == userId);
            if (todoList == null)
            {
                throw new ArgumentException("Todo list not found or does not belong to the user.", nameof(taskItemId));
            }

            var entity = await this.context.TaskItems.FindAsync(taskItemId);
            if (entity == null)
            {
                return false;
            }

            _ = this.context.TaskItems.Remove(entity);
            _ = await this.context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TaskItem>> SearchAsync(int userId, string title = null, DateTime? createdAt = null, DateTime? dueDate = null)
        {
            var todoLists = await this.context.TodoLists
                .Where(t => t.OwnerId == userId)
                .Select(t => t.Id)
                .ToListAsync();

            var query = this.context.TaskItems.Where(t => todoLists.Contains(t.TodoListId)).AsQueryable();

            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(t => t.Title.Contains(title));
            }

            if (createdAt.HasValue)
            {
                query = query.Where(t => t.CreatedAt.Date == createdAt.Value.Date);
            }

            if (dueDate.HasValue)
            {
                query = query.Where(t => t.DueDate.HasValue && t.DueDate.Value.Date == dueDate.Value.Date);
            }

            var results = await query.ToListAsync();
            return results.Select(TaskItemMapper.ToModel);
        }

        public async Task<bool> ChangeTaskStatusAsync(int userId, int taskId, string status)
        {
            var todoLists = await this.context.TodoLists
                .Where(t => t.OwnerId == userId)
                .Select(t => t.Id)
                .ToListAsync();

            var tasks = this.context.TaskItems.Where(t => todoLists.Contains(t.TodoListId)).AsQueryable();

            var task = await tasks.FirstOrDefaultAsync(t => t.Id == taskId);

            if (Enum.TryParse<Models.TaskStatus>(status, out var newStatus))
            {
                task.Status = newStatus.ToString();
            }
            else
            {
                throw new ArgumentException("Invalid status value.");
            }
            _ = await this.context.SaveChangesAsync();

            return true;
        }
    }
}
