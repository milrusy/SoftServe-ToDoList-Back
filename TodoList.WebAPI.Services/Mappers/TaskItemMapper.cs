using TodoList.WebAPI.Models;
using TodoList.WebAPI.Models.DTOs;
using TodoList.WebAPI.Services.Database.Entities;

namespace TodoList.WebAPI.Services.Mappers
{
    public static class TaskItemMapper
    {
        public static TaskItemEntity ToEntity(TaskItem model)
        {
            return new TaskItemEntity()
            {
                Id = model.Id,
                Title = model.Title,
                Description = model.Description,
                Status = model.Status.ToString(),
                CreatedAt = model.CreatedAt,
                DueDate = model.DueDate,
                TodoListId = model.TodoListId,
            };
        }

        public static TaskItem ToModel(TaskItemEntity entity)
        {
            return new TaskItem()
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                Status = Enum.Parse<Models.TaskStatus>(entity.Status),
                CreatedAt = entity.CreatedAt,
                DueDate = entity.DueDate,
                TodoListId = entity.TodoListId,
            };
        }

        public static TaskItem ToModel(TaskItemDto dto)
        {
            return new TaskItem()
            {
                Id = dto.Id,
                Title = dto.Title,
                Description = dto.Description,
                Status = Enum.Parse<Models.TaskStatus>(dto.Status),
                CreatedAt = dto.CreatedAt,
                DueDate = dto.DueDate,
                TodoListId = dto.TodoListId,
            };
        }

        public static TaskItemDto ToDto(TaskItem model)
        {
            return new TaskItemDto()
            {
                Id = model.Id,
                Title = model.Title,
                Description = model.Description,
                Status = model.Status.ToString(),
                CreatedAt = model.CreatedAt,
                DueDate = model.DueDate,
                TodoListId = model.TodoListId,
            };
        }

    }
}
