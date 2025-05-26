using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoList.WebAPI.Models;
using TodoList.WebAPI.Services;
using TodoList.WebAPI.Services.Database.Contexts;
using TodoList.WebAPI.Services.Database.Entities;
using Xunit;

public class TaskItemDatabaseServiceTests
{
    private async Task<TodoListDbContext> GetInMemoryDbContextAsync()
    {
        var options = new DbContextOptionsBuilder<TodoListDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new TodoListDbContext(options);
        await context.Database.EnsureCreatedAsync();
        return context;
    }

    [Fact]
    public async Task CreateAsync_CreatesTaskForValidTodoList()
    {
        var context = await GetInMemoryDbContextAsync();
        context.TodoLists.Add(new TodoListEntity { Id = 5, Title = "CreateTest", OwnerId = 99 });
        await context.SaveChangesAsync();

        var service = new TaskItemDatabaseService(context);
        var task = new TaskItem { Title = "Test Task", Description = "Test Desc" };

        var result = await service.CreateAsync(99, 5, task);

        Assert.NotNull(result);
        Assert.Equal("Test Task", result.Title);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsTaskIfBelongsToUser()
    {
        var context = await GetInMemoryDbContextAsync();
        context.TodoLists.Add(new TodoListEntity { Id = 5, Title = "Test Title", OwnerId = 1 });
        context.TaskItems.Add(new TaskItemEntity { Id = 100, Title = "Task1", Status = "NotStarted", TodoListId = 5, CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var service = new TaskItemDatabaseService(context);
        var result = await service.GetByIdAsync(1, 100);

        Assert.NotNull(result);
        Assert.Equal("Task1", result.Title);
    }

    [Fact]
    public async Task DeleteAsync_DeletesTaskIfOwnedByUser()
    {
        var context = await GetInMemoryDbContextAsync();
        context.TodoLists.Add(new TodoListEntity { Id = 15, Title = "Test Title", OwnerId = 10 });
        context.TaskItems.Add(new TaskItemEntity { Id = 200, Title = "DeleteMe", Status = "NotStarted", TodoListId = 15});
        await context.SaveChangesAsync();

        var service = new TaskItemDatabaseService(context);
        var result = await service.DeleteAsync(10, 200);

        Assert.True(result);
    }

    [Fact]
    public async Task ChangeTaskStatusAsync_UpdatesStatus()
    {
        var context = await GetInMemoryDbContextAsync();
        var list = new TodoListEntity { Id = 7, Title = "StatusTest", OwnerId = 777 };
        var task = new TaskItemEntity { Id = 70, Title = "StatusTest", TodoListId = 7, Status = "Pending" };
        context.TodoLists.Add(list);
        context.TaskItems.Add(task);
        await context.SaveChangesAsync();

        var service = new TaskItemDatabaseService(context);
        var result = await service.ChangeTaskStatusAsync(777, 70, "Completed");

        Assert.True(result);
        var updated = await context.TaskItems.FindAsync(70);
        Assert.Equal("Completed", updated.Status);
    }
}
