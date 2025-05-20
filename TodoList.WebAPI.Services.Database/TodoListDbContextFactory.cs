using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.SqlServer;
using TodoList.WebAPI.Services.Database.Contexts;

namespace TodoList.WebAPI.Services.Database
{
    public class TodoListDbContextFactory : IDesignTimeDbContextFactory<TodoListDbContext>
    {
        public TodoListDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TodoListDbContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=TodoListAppDb;Trusted_Connection=True;");
            return new TodoListDbContext(optionsBuilder.Options);
        }
    }
}
