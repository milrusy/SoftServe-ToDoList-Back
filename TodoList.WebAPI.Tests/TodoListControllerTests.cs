using Moq;
using Microsoft.AspNetCore.Mvc;
using TodoList.WebAPI.Controllers;
using TodoList.WebAPI.Services.Interfaces;
using TodoList.WebAPI.Models.DTOs;

namespace TodoList.Tests.Controllers
{
    public class TodoListControllerTests
    {
        private readonly Mock<ITodoListDatabaseService> serviceMock;
        private readonly Mock<IAuthorizationService> authMock;
        private readonly TodoListController controller;

        public TodoListControllerTests()
        {
            this.serviceMock = new Mock<ITodoListDatabaseService>();
            this.authMock = new Mock<IAuthorizationService>();
            this.controller = new TodoListController(this.serviceMock.Object, this.authMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOkResult_WithTodoListDtos()
        {
            // Arrange
            var userId = 123;
            this.authMock.Setup(a => a.GetUserId()).Returns(userId);
            var todoLists = new List<WebAPI.Models.TodoList>
            {
                new WebAPI.Models.TodoList
                {
                    Id = 1,
                    Title = "Test",
                    Description = "Test",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    OwnerId = userId,
                }
            };
            this.serviceMock.Setup(s => s.GetAllAsync(userId)).ReturnsAsync(todoLists);

            // Act
            var result = await this.controller.GetAllAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<TodoListDto>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsNotFound_WhenServiceReturnsNull()
        {
            // Arrange
            var userId = 123;
            this.authMock.Setup(a => a.GetUserId()).Returns(userId);
            this.serviceMock.Setup(s => s.GetAllAsync(userId)).ReturnsAsync((List<WebAPI.Models.TodoList>?)null);

            // Act
            var result = await this.controller.GetAllAsync();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsOk_WhenTodoListExists()
        {
            // Arrange
            var userId = 123;
            var todo = new WebAPI.Models.TodoList { Id = 1, Title = "Test", OwnerId = userId };
            this.authMock.Setup(a => a.GetUserId()).Returns(userId);
            this.serviceMock.Setup(s => s.GetByIdAsync(userId, 1)).ReturnsAsync(todo);

            // Act
            var result = await this.controller.GetByIdAsync(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<TodoListDto>(okResult.Value);
            Assert.Equal(1, returnValue.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNotFound_WhenTodoListNotExists()
        {
            // Arrange
            this.authMock.Setup(a => a.GetUserId()).Returns(123);
            this.serviceMock.Setup(s => s.GetByIdAsync(123, 99)).ReturnsAsync((WebAPI.Models.TodoList?)null);

            // Act
            var result = await this.controller.GetByIdAsync(99);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateAsync_ReturnsOk_WhenValidDto()
        {
            // Arrange
            var userId = 123;
            var dto = new TodoListDto { Id = 0, Title = "New List" };
            var created = new WebAPI.Models.TodoList
            {
                Id = 1,
                Title = "Test",
                Description = "Test",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                OwnerId = userId,
            };

            this.authMock.Setup(a => a.GetUserId()).Returns(userId);
            this.serviceMock.Setup(s => s.CreateAsync(userId, It.IsAny<WebAPI.Models.TodoList>()))
                        .ReturnsAsync(created);

            // Act
            var result = await this.controller.CreateAsync(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<TodoListDto>(okResult.Value);
            Assert.Equal(1, returnValue.Id);
        }

        [Fact]
        public async Task CreateAsync_ReturnsBadRequest_WhenDtoIsNull()
        {
            // Act
            var result = await this.controller.CreateAsync(null);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsOk_WhenUpdateSuccessful()
        {
            // Arrange
            var userId = 123;
            var dto = new TodoListDto { Id = 1, Title = "Updated" };
            var updated = new WebAPI.Models.TodoList
            {
                Id = 1,
                Title = "Updated",
                OwnerId = userId,
            };

            this.authMock.Setup(a => a.GetUserId()).Returns(userId);
            this.serviceMock.Setup(s => s.UpdateAsync(userId, 1, It.IsAny<WebAPI.Models.TodoList>()))
                        .ReturnsAsync(updated);

            // Act
            var result = await this.controller.UpdateAsync(1, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<TodoListDto>(okResult.Value);
            Assert.Equal("Updated", returnValue.Title);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNotFound_WhenUpdateFails()
        {
            // Arrange
            var dto = new TodoListDto { Id = 1, Title = "Does Not Exist" };
            this.authMock.Setup(a => a.GetUserId()).Returns(123);
            this.serviceMock.Setup(s => s.UpdateAsync(123, 1, It.IsAny<WebAPI.Models.TodoList>()))
                        .ReturnsAsync((WebAPI.Models.TodoList?)null);

            // Act
            var result = await this.controller.UpdateAsync(1, dto);

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
            this.authMock.Setup(a => a.GetUserId()).Returns(123);
            this.serviceMock.Setup(s => s.DeleteAsync(123, 1)).ReturnsAsync(true);

            // Act
            var result = await this.controller.DeleteAsync(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNotFound_WhenNotDeleted()
        {
            // Arrange
            this.authMock.Setup(a => a.GetUserId()).Returns(123);
            this.serviceMock.Setup(s => s.DeleteAsync(123, 1)).ReturnsAsync(false);

            // Act
            var result = await this.controller.DeleteAsync(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
