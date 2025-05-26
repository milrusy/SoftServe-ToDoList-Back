using Microsoft.EntityFrameworkCore;
using TodoList.WebAPI.Services;
using TodoList.WebAPI.Services.Database.Contexts;
using TodoList.WebAPI.Services.Database.Entities;

public class TodoListDatabaseServiceTests
{
    private TodoListDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<TodoListDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new TodoListDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_AddsTodoList()
    {
        var context = GetInMemoryDbContext();
        var service = new TodoListDatabaseService(context);
        var userId = 1;
        var todoList = new TodoList.WebAPI.Models.TodoList
        {
            Title = "Test",
            Description = "Test description"
        };

        var result = await service.CreateAsync(userId, todoList);

        Assert.NotNull(result);
        Assert.Equal(userId, result.OwnerId);
        Assert.Equal(todoList.Title, result.Title);
        Assert.NotEqual(default, result.CreatedAt);
        Assert.NotEqual(default, result.UpdatedAt);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsTodoList_IfOwnedByUser()
    {
        var context = GetInMemoryDbContext();
        var userId = 1;
        var todoListEntity = new TodoListEntity
        {
            Id = 1,
            OwnerId = userId,
            Title = "List 1",
            Description = "Desc"
        };
        context.TodoLists.Add(todoListEntity);
        await context.SaveChangesAsync();

        var service = new TodoListDatabaseService(context);

        var result = await service.GetByIdAsync(userId, todoListEntity.Id);

        Assert.NotNull(result);
        Assert.Equal(todoListEntity.Id, result.Id);
        Assert.Equal(todoListEntity.Title, result.Title);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_IfNotOwnedByUser()
    {
        var context = GetInMemoryDbContext();
        var userId = 1;
        var todoListEntity = new TodoListEntity
        {
            Id = 1,
            OwnerId = 2,
            Title = "List 1",
            Description = "Desc"
        };
        context.TodoLists.Add(todoListEntity);
        await context.SaveChangesAsync();

        var service = new TodoListDatabaseService(context);

        var result = await service.GetByIdAsync(userId, todoListEntity.Id);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_RemovesTodoList_IfExists()
    {
        var context = GetInMemoryDbContext();
        var userId = 1;
        var todoListEntity = new TodoListEntity
        {
            Id = 1,
            OwnerId = userId,
            Title = "List to delete",
            Description = "Desc"
        };
        context.TodoLists.Add(todoListEntity);
        await context.SaveChangesAsync();

        var service = new TodoListDatabaseService(context);

        var result = await service.DeleteAsync(userId, todoListEntity.Id);

        Assert.True(result);
        Assert.False(context.TodoLists.Any(t => t.Id == todoListEntity.Id));
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_IfNotExists()
    {
        var context = GetInMemoryDbContext();
        var service = new TodoListDatabaseService(context);
        var result = await service.DeleteAsync(1, 999);
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesTodoList_IfExists()
    {
        var context = GetInMemoryDbContext();
        var userId = 1;
        var todoListEntity = new TodoListEntity
        {
            Id = 1,
            OwnerId = userId,
            Title = "Old Title",
            Description = "Old Desc"
        };
        context.TodoLists.Add(todoListEntity);
        await context.SaveChangesAsync();

        var service = new TodoListDatabaseService(context);

        var updatedModel = new TodoList.WebAPI.Models.TodoList
        {
            Title = "New Title",
            Description = "New Desc"
        };

        var result = await service.UpdateAsync(userId, todoListEntity.Id, updatedModel);

        Assert.NotNull(result);
        Assert.Equal("New Title", result.Title);
        Assert.Equal("New Desc", result.Description);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_IfNotExists()
    {
        var context = GetInMemoryDbContext();
        var service = new TodoListDatabaseService(context);

        var updatedModel = new TodoList.WebAPI.Models.TodoList
        {
            Title = "New Title",
            Description = "New Desc"
        };

        var result = await service.UpdateAsync(1, 999, updatedModel);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyUsersTodoLists()
    {
        var context = GetInMemoryDbContext();

        context.TodoLists.Add(new TodoListEntity
        {
            Id = 1,
            OwnerId = 1,
            Title = "User1 List"
        });

        context.TodoLists.Add(new TodoListEntity
        {
            Id = 2,
            OwnerId = 2,
            Title = "User2 List"
        });

        await context.SaveChangesAsync();

        var service = new TodoListDatabaseService(context);

        var result = (await service.GetAllAsync(1)).ToList();

        Assert.Single(result);
        Assert.Equal("User1 List", result[0].Title);
    }
}
