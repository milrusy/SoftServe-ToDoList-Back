using Microsoft.EntityFrameworkCore;
using TodoList.WebAPI.Services.Database.Contexts;
using TodoList.WebAPI.Services.Interfaces;
using TodoList.WebAPI.Services.Mappers;

namespace TodoList.WebAPI.Services
{
    public class TodoListDatabaseService : ITodoListDatabaseService
    {
        private readonly TodoListDbContext context;

        public TodoListDatabaseService(TodoListDbContext context)
        {
            this.context = context;
        }

        public async Task<Models.TodoList> GetByIdAsync(int id)
        {
            var todoList = await this.context.TodoLists.FirstOrDefaultAsync(t => t.Id == id);
            if (todoList == null)
            {
                return null;
            }

            return TodoListMapper.ToModel(todoList);
        }

        public async Task<IEnumerable<Models.TodoList>> GetAllAsync()
        {
            var todoLists = await this.context.TodoLists.ToListAsync();
            return todoLists.Select(t => TodoListMapper.ToModel(t));
        }

        public async Task<Models.TodoList> CreateAsync(Models.TodoList todoList)
        {
            todoList.CreatedAt = DateTime.UtcNow;
            todoList.UpdatedAt = DateTime.UtcNow;
            var entity = this.context.TodoLists.Add(TodoListMapper.ToEntity(todoList));
            _ = await this.context.SaveChangesAsync();
            return TodoListMapper.ToModel(entity.Entity);
        }

        public async Task<Models.TodoList> UpdateAsync(int id, Models.TodoList todoList)
        {
            var entity = await this.context.TodoLists.FindAsync(id);
            if (entity == null)
            {
                return null;
            }

            entity.Title = todoList.Title;
            entity.Description = todoList.Description;
            entity.UpdatedAt = DateTime.UtcNow;

            _ = await this.context.SaveChangesAsync();
            return TodoListMapper.ToModel(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await this.context.TodoLists.FindAsync(id);
            if (entity == null)
            {
                return false;
            }

            _ = this.context.TodoLists.Remove(entity);
            _ = await this.context.SaveChangesAsync();
            return true;
        }
    }
}
