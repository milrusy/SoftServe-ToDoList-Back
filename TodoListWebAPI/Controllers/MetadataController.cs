using Microsoft.AspNetCore.Mvc;
using TodoList.WebAPI.Models.DTOs;

namespace TodoList.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MetadataController : ControllerBase
    {
        [HttpGet("taskStatuses")]
        public IActionResult GetTaskStatuses()
        {
            var values = Enum.GetValues(typeof(Models.TaskStatus))
                .Cast<Models.TaskStatus>()
                .Select(e => new TaskStatusDto
                {
                    Name = e.ToString(),
                    Value = (int)e
                });

            return Ok(values);
        }
    }

}
