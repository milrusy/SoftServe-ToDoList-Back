using Microsoft.EntityFrameworkCore;
using TodoList.WebAPI.Services.Database.Contexts;
using TodoList.WebAPI.Services.Interfaces;
using TodoList.WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ITodoListDatabaseService, TodoListDatabaseService>();
builder.Services.AddScoped<ITaskItemDatabaseService, TaskItemDatabaseService>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TodoListDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TodoListDbContext")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
