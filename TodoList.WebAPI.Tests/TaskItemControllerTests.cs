using Xunit;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TodoList.WebAPI.Controllers;
using TodoList.WebAPI.Services.Interfaces;
using TodoList.WebAPI.Models.DTOs;
using TodoList.WebAPI.Models;
using TodoList.WebAPI.Services.Mappers;

namespace TodoList.WebAPI.Tests
{
    public class TaskItemControllerTests
    {
        private readonly Mock<ITaskItemDatabaseService> serviceMock;
        private readonly Mock<IAuthorizationService> authMock;
        private readonly TaskItemController controller;

        public TaskItemControllerTests()
        {
            this.serviceMock = new Mock<ITaskItemDatabaseService>();
            this.authMock = new Mock<IAuthorizationService>();
            this.controller = new TaskItemController(serviceMock.Object, authMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOkResult_WithTaskItemDtos()
        {
            // Arrange
            int todoListId = 1;
            int userId = 123;
            this.authMock.Setup(a => a.GetUserId()).Returns(userId);
            var taskItems = new List<TaskItem>
            {
                new TaskItem { Id = 1, Title = "Task 1", TodoListId = todoListId },
                new TaskItem { Id = 2, Title = "Task 2", TodoListId = todoListId }
            };
            this.serviceMock.Setup(s => s.GetAllByTodoListIdAsync(userId, todoListId))
                .ReturnsAsync(taskItems);

            // Act
            var result = await this.controller.GetAllAsync(todoListId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var taskItemDtos = Assert.IsAssignableFrom<List<TaskItemDto>>(okResult.Value);
            Assert.Equal(2, taskItemDtos.Count);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsNotFound_WhenServiceReturnsNull()
        {
            // Arrange
            var userId = 123;
            this.authMock.Setup(a => a.GetUserId()).Returns(userId);
            this.serviceMock.Setup(s => s.GetAllByTodoListIdAsync(userId, It.IsAny<int>()))
                .ReturnsAsync((List<TaskItem>)null);

            // Act
            var result = await this.controller.GetAllAsync(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsBadRequest_WhenTodoListIdIsNull()
        {
            // Act
            var result = await this.controller.GetAllAsync(null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsOk_WhenTaskItemExists()
        {
            // Arrange
            int taskItemId = 1;
            int userId = 123;
            this.authMock.Setup(a => a.GetUserId()).Returns(userId);
            var taskItem = new TaskItem { Id = taskItemId, Title = "Task 1" };
            this.serviceMock.Setup(s => s.GetByIdAsync(userId, taskItemId))
                .ReturnsAsync(taskItem);

            // Act
            var result = await this.controller.GetByIdAsync(taskItemId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var taskItemDto = Assert.IsType<TaskItemDto>(okResult.Value);
            Assert.Equal(taskItemId, taskItemDto.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNotFound_WhenTaskItemDoesNotExist()
        {
            // Arrange
            int taskItemId = 1;
            int userId = 123;
            this.authMock.Setup(a => a.GetUserId()).Returns(userId);
            this.serviceMock.Setup(s => s.GetByIdAsync(userId, taskItemId))
                .ReturnsAsync((TaskItem)null);

            // Act
            var result = await this.controller.GetByIdAsync(taskItemId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateAsync_ReturnsOk_WhenValidDto()
        {
            // Arrange
            int todoListId = 1;
            int userId = 123;
            this.authMock.Setup(a => a.GetUserId()).Returns(userId);
            var taskItemDto = new TaskItemDto
            {
                Title = "New Task",
                Description = "New",
                Status = "NotStarted",
            };

            var taskItem = new TaskItem
            {
                Id = 1,
                Title = "New Task",
                Description = "New",
                Status = Models.TaskStatus.NotStarted,
            };
            taskItem.TodoListId = todoListId;
            this.serviceMock
                .Setup(s => s.CreateAsync(userId, todoListId, It.IsAny<TaskItem>()))
                .ReturnsAsync(taskItem);

            // Act
            var result = await this.controller.CreateAsync(todoListId, taskItemDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var createdTaskItemDto = Assert.IsType<TaskItemDto>(okResult.Value);
            Assert.Equal(taskItemDto.Title, createdTaskItemDto.Title);
        }

        [Fact]
        public async Task CreateAsync_ReturnsBadRequest_WhenDtoIsNull()
        {
            // Act
            var result = await this.controller.CreateAsync(1, null);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsOk_WhenUpdateSuccessful()
        {
            // Arrange
            int userId = 123;
            this.authMock.Setup(a => a.GetUserId()).Returns(userId);
            this.serviceMock.Setup(s => s.UpdateAsync(userId, 1, It.IsAny<TaskItem>()))
                   .ReturnsAsync(new TaskItem { Id = 1, Title = "Updated", Description = "Updated", Status = Models.TaskStatus.NotStarted });

            // Act
            var result = await controller.UpdateAsync(1, new TaskItemDto { Title = "Updated", Description = "Updated", Status = "NotStarted" }) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TaskItemDto>(result.Value);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNotFound_WhenUpdateFails()
        {
            // Arrange
            int userId = 123;
            this.serviceMock.Setup(s => s.UpdateAsync(userId, 1, It.IsAny<TaskItem>())).ReturnsAsync((TaskItem?)null);

            // Act
            var result = await controller.UpdateAsync(1, new TaskItemDto { Title = "Updated", Description = "Updated", Status = "NotStarted" });

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsBadRequest_WhenDtoIsNull()
        {
            // Act
            var result = await this.controller.UpdateAsync(1, null);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNoContent_WhenDeleted()
        {
            // Arrange
            int taskItemId = 1;
            int userId = 123;
            this.authMock.Setup(a => a.GetUserId()).Returns(userId);
            this.serviceMock.Setup(s => s.DeleteAsync(userId, taskItemId))
                .ReturnsAsync(true);

            // Act
            var result = await this.controller.DeleteAsync(taskItemId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNotFound_WhenNotDeleted()
        {
            // Arrange
            int taskItemId = 1;
            int userId = 123;
            this.authMock.Setup(a => a.GetUserId()).Returns(userId);
            this.serviceMock.Setup(s => s.DeleteAsync(userId, taskItemId))
                .ReturnsAsync(false);

            // Act
            var result = await this.controller.DeleteAsync(taskItemId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
