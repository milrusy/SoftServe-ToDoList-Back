using Microsoft.AspNetCore.Mvc;
using TodoList.WebAPI.Models.DTOs;
using TodoList.WebAPI.Services.Interfaces;
using TodoList.WebAPI.Services.Mappers;

namespace TodoList.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoListController : ControllerBase
    {
        private readonly ITodoListDatabaseService service;

        public TodoListController(ITodoListDatabaseService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var todoListsModels = await this.service.GetAllAsync();

            if (todoListsModels == null)
            {
                return this.NotFound();
            }

            var todoListsDtos = todoListsModels.Select(m =>
            {
                return TodoListMapper.ToDto(m);
            }).ToList();

            return this.Ok(todoListsDtos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var todoList = await this.service.GetByIdAsync(id);
            if (todoList == null)
            {
                return this.NotFound();
            }

            return this.Ok(TodoListMapper.ToDto(todoList));
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] TodoListDto todoListDto)
        {
            if (todoListDto == null)
            {
                return this.BadRequest();
            }

            var createdTodoList = await this.service.CreateAsync(TodoListMapper.ToModel(todoListDto));
            return this.Ok(TodoListMapper.ToDto(createdTodoList));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] TodoListDto todoListDto)
        {
            if (todoListDto == null)
            {
                return this.BadRequest();
            }

            var updatedTodoList = await this.service.UpdateAsync(id, TodoListMapper.ToModel(todoListDto));
            if (updatedTodoList == null)
            {
                return this.NotFound();
            }

            return this.Ok(TodoListMapper.ToDto(updatedTodoList));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deleted = await this.service.DeleteAsync(id);
            if (!deleted)
            {
                return this.NotFound();
            }

            return this.NoContent();
        }
    }
}
