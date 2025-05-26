using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoList.WebAPI.Models.DTOs;
using TodoList.WebAPI.Services.Interfaces;
using TodoList.WebAPI.Services.Mappers;

namespace TodoList.WebAPI.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class TaskItemController : ControllerBase
    {
        private readonly ITaskItemDatabaseService service;
        private readonly Services.Interfaces.IAuthorizationService authorizationService;

        public TaskItemController(ITaskItemDatabaseService service, Services.Interfaces.IAuthorizationService authorizationService)
        {
            this.service = service;
            this.authorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] int? todoListId = null)
        {
            if (todoListId == null)
            {
                return this.BadRequest("Missing todoListId query parameter.");
            }

            var userId = this.authorizationService.GetUserId();

            var taskItems = await this.service.GetAllByTodoListIdAsync(userId, (int)todoListId);
            if (taskItems == null)
            {
                return this.NotFound();
            }

            var taskItemsDtos = taskItems.Select(t => TaskItemMapper.ToDto(t)).ToList();

            return this.Ok(taskItemsDtos);
        }

        [HttpGet("{taskItemId:int}")]
        public async Task<IActionResult> GetByIdAsync(int taskItemId)
        {
            var userId = this.authorizationService.GetUserId();
            var taskItem = await this.service.GetByIdAsync(userId, taskItemId);
            if (taskItem == null)
            {
                return this.NotFound();
            }

            return this.Ok(TaskItemMapper.ToDto(taskItem));
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromQuery] int todoListId, [FromBody] TaskItemDto taskItemDto)
        {
            var userId = this.authorizationService.GetUserId();
            if (taskItemDto == null)
            {
                return this.BadRequest();
            }

            var createdTaskItem = await this.service.CreateAsync(userId, todoListId, TaskItemMapper.ToModel(taskItemDto));
            return this.Ok(TaskItemMapper.ToDto(createdTaskItem));
        }

        [HttpPut("{taskItemId:int}")]
        public async Task<IActionResult> UpdateAsync(int taskItemId, [FromBody] TaskItemDto taskItemDto)
        {
            var userId = this.authorizationService.GetUserId();
            if (taskItemDto == null)
            {
                return this.BadRequest();
            }

            var updatedTaskItem = await this.service.UpdateAsync(userId, taskItemId, TaskItemMapper.ToModel(taskItemDto));
            if (updatedTaskItem == null)
            {
                return this.NotFound();
            }

            return this.Ok(TaskItemMapper.ToDto(updatedTaskItem));
        }

        [HttpDelete("{taskItemId:int}")]
        public async Task<IActionResult> DeleteAsync(int taskItemId)
        {
            var userId = this.authorizationService.GetUserId();
            var isDeleted = await this.service.DeleteAsync(userId, taskItemId);
            if (!isDeleted)
            {
                return this.NotFound();
            }

            return this.NoContent();
        }
    }
}
