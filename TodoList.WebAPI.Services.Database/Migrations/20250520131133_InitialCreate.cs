using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoList.WebAPI.Services.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "TodoLists",
                columns: table => new
                {
                    TodoListId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OwnerId = table.Column<int>(type: "int", nullable: false),
                    UserEntityId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodoLists", x => x.TodoListId);
                    table.ForeignKey(
                        name: "FK_TodoLists_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TodoLists_Users_UserEntityId",
                        column: x => x.UserEntityId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "TaskItems",
                columns: table => new
                {
                    TaskItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TodoListId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskItems", x => x.TaskItemId);
                    table.ForeignKey(
                        name: "FK_TaskItems_TodoLists_TodoListId",
                        column: x => x.TodoListId,
                        principalTable: "TodoLists",
                        principalColumn: "TodoListId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Email", "PasswordHash", "Username" },
                values: new object[] { 1, "admin@example.com", "hashed-password", "admin" });

            migrationBuilder.InsertData(
                table: "TodoLists",
                columns: new[] { "TodoListId", "CreatedAt", "Description", "OwnerId", "Title", "UpdatedAt", "UserEntityId" },
                values: new object[] { 1, new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "This is a seeded todo list", 1, "First List", new DateTime(2024, 1, 2, 12, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "TaskItems",
                columns: new[] { "TaskItemId", "CreatedAt", "Description", "DueDate", "Status", "Title", "TodoListId" },
                values: new object[] { 1, new DateTime(2024, 1, 3, 9, 0, 0, 0, DateTimeKind.Utc), "This is a seeded task", new DateTime(2024, 1, 10, 9, 0, 0, 0, DateTimeKind.Utc), "Pending", "First Task", 1 });

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_TodoListId",
                table: "TaskItems",
                column: "TodoListId");

            migrationBuilder.CreateIndex(
                name: "IX_TodoLists_OwnerId",
                table: "TodoLists",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_TodoLists_UserEntityId",
                table: "TodoLists",
                column: "UserEntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskItems");

            migrationBuilder.DropTable(
                name: "TodoLists");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
