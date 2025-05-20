using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoList.WebAPI.Models.DTOs;
using TodoList.WebAPI.Services.Database.Entities;

namespace TodoList.WebAPI.Services.Mappers
{
    public static class TodoListMapper
    {
        public static TodoListEntity ToEntity(Models.TodoList model) =>
            new()
            {
                Id = model.Id,
                Title = model.Title,
                Description = model.Description,
                CreatedAt = model.CreatedAt,
                UpdatedAt = model.UpdatedAt,
                OwnerId = model.OwnerId,
            };

        public static Models.TodoList ToModel(TodoListEntity entity) =>
            new()
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                OwnerId = entity.OwnerId,
                OwnerUsername = entity.Owner?.Username,
                Tasks = entity.Tasks.Select(TaskItemMapper.ToModel).ToList(),
            };

        public static Models.TodoList ToModel(TodoListDto dto) =>
        new()
        {
            Id = dto.Id,
            Title = dto.Title,
            Description = dto.Description,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            OwnerId = dto.OwnerId,
        };

        public static TodoListDto ToDto(Models.TodoList model) =>
            new()
            {
                Id = model.Id,
                Title = model.Title,
                Description = model.Description,
                CreatedAt = model.CreatedAt,
                UpdatedAt = model.UpdatedAt,
                OwnerId = model.OwnerId,
            };

    }
}
