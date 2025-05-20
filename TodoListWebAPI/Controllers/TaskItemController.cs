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

        public TaskItemController(ITaskItemDatabaseService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] int? todoListId = null)
        {
            if (todoListId == null)
            {
                return this.BadRequest("Missing todoListId query parameter.");
            }

            var taskItems = await this.service.GetAllByTodoListIdAsync((int)todoListId);
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
            var taskItem = await this.service.GetByIdAsync(taskItemId);
            if (taskItem == null)
            {
                return this.NotFound();
            }

            return this.Ok(TaskItemMapper.ToDto(taskItem));
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromQuery] int todoListId, [FromBody] TaskItemDto taskItemDto)
        {
            if (taskItemDto == null)
            {
                return this.BadRequest();
            }

            var createdTaskItem = await this.service.CreateAsync(todoListId, TaskItemMapper.ToModel(taskItemDto));
            return this.Ok(TaskItemMapper.ToDto(createdTaskItem));
        }

        [HttpPut("{taskItemId:int}")]
        public async Task<IActionResult> UpdateAsync(int taskItemId, [FromBody] TaskItemDto taskItemDto)
        {
            if (taskItemDto == null)
            {
                return this.BadRequest();
            }

            var updatedTaskItem = await this.service.UpdateAsync(taskItemId, TaskItemMapper.ToModel(taskItemDto));
            if (updatedTaskItem == null)
            {
                return this.NotFound();
            }

            return this.Ok(TaskItemMapper.ToDto(updatedTaskItem));
        }

        [HttpDelete("{taskItemId:int}")]
        public async Task<IActionResult> DeleteAsync(int taskItemId)
        {
            var isDeleted = await this.service.DeleteAsync(taskItemId);
            if (!isDeleted)
            {
                return this.NotFound();
            }

            return this.NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchAsync(
        [FromQuery] string? title,
        [FromQuery] DateTime? createdAt,
        [FromQuery] DateTime? dueDate)
        {
            var results = await this.service.SearchAsync(title, createdAt, dueDate);
            var resultDtos = results.Select(TaskItemMapper.ToDto).ToList();

            return this.Ok(resultDtos);
        }
    }
}
