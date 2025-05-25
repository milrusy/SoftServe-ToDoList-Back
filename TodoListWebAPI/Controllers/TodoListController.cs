using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoList.WebAPI.Models.DTOs;
using TodoList.WebAPI.Services.Interfaces;
using TodoList.WebAPI.Services.Mappers;

namespace TodoList.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TodoListController : ControllerBase
    {
        private readonly ITodoListDatabaseService service;
        private readonly Services.Interfaces.IAuthorizationService authorizationService;

        public TodoListController(ITodoListDatabaseService service, Services.Interfaces.IAuthorizationService authorizationService)
        {
            this.service = service;
            this.authorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var userId = this.authorizationService.GetUserId();
            var todoListsModels = await this.service.GetAllAsync(userId);

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
            var userId = this.authorizationService.GetUserId();
            var todoList = await this.service.GetByIdAsync(userId, id);
            if (todoList == null)
            {
                return this.NotFound();
            }

            return this.Ok(TodoListMapper.ToDto(todoList));
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] TodoListDto todoListDto)
        {
            var userId = this.authorizationService.GetUserId();

            if (todoListDto == null)
            {
                return this.BadRequest();
            }

            var createdTodoList = await this.service.CreateAsync(userId, TodoListMapper.ToModel(todoListDto));
            return this.Ok(TodoListMapper.ToDto(createdTodoList));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] TodoListDto todoListDto)
        {
            var userId = this.authorizationService.GetUserId();

            if (todoListDto == null)
            {
                return this.BadRequest();
            }

            var updatedTodoList = await this.service.UpdateAsync(userId, id, TodoListMapper.ToModel(todoListDto));
            if (updatedTodoList == null)
            {
                return this.NotFound();
            }

            return this.Ok(TodoListMapper.ToDto(updatedTodoList));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var userId = this.authorizationService.GetUserId();
            var deleted = await this.service.DeleteAsync(userId, id);
            if (!deleted)
            {
                return this.NotFound();
            }

            return this.NoContent();
        }
    }
}
