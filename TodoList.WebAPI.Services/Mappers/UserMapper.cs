using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoList.WebAPI.Models;
using TodoList.WebAPI.Services.Database.Entities;

namespace TodoList.WebAPI.Services.Mappers
{
    public static class UserMapper
    {
        public static UserEntity ToEntity(User model)
        {
            return new UserEntity()
            {
                Id = model.Id,
                Username = model.Username,
                Email = model.Email,
            };
        }

        public static User ToModel(UserEntity entity)
        {
            return new User()
            {
                Id = entity.Id,
                Username = entity.Username,
                Email = entity.Email,
                TodoLists = entity is not null ? entity.TodoLists.Select(TodoListMapper.ToModel).ToList() : null,
            };
        }
    }

}
