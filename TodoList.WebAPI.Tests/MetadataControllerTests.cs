using Microsoft.AspNetCore.Mvc;
using TodoList.WebAPI.Controllers;
using TodoList.WebAPI.Models.DTOs;

namespace TodoList.WebAPI.Tests
{
    public class MetadataControllerTests
    {
        [Fact]
        public void GetTaskStatuses_ReturnsOkResult_WithExpectedStatuses()
        {
            // Arrange
            var controller = new MetadataController();

            // Act
            var result = controller.GetTaskStatuses();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var statuses = Assert.IsAssignableFrom<IEnumerable<TaskStatusDto>>(okResult.Value);

            var statusList = statuses.ToList();
            Assert.Equal(Enum.GetValues(typeof(Models.TaskStatus)).Length, statusList.Count);

            foreach (var status in Enum.GetValues(typeof(Models.TaskStatus)).Cast<Models.TaskStatus>())
            {
                Assert.Contains(statusList, s => s.Name == status.ToString() && s.Value == (int)status);
            }
        }
    }
}
