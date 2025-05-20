using Microsoft.EntityFrameworkCore;
using System;
using TodoList.WebAPI.Services.Database.Entities;

namespace TodoList.WebAPI.Services.Database.Contexts
{
    public class TodoListDbContext : DbContext
    {
        public TodoListDbContext(DbContextOptions<TodoListDbContext> options)
            : base(options)
        {
        }
        public DbSet<TodoListEntity> TodoLists { get; set; }

        public DbSet<TaskItemEntity> TaskItems { get; set; }

        public DbSet<UserEntity> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<UserEntity>(entity =>
            {
                _ = entity.HasKey(e => e.Id);
                _ = entity.Property(e => e.Username).IsRequired();
                _ = entity.Property(e => e.Email).IsRequired();
                _ = entity.Property(e => e.PasswordHash).IsRequired();
            });

            _ = modelBuilder.Entity<TodoListEntity>(entity =>
            {
                _ = entity.HasKey(e => e.Id);
                _ = entity.Property(e => e.Title).IsRequired();
                _ = entity.Property(e => e.Description).IsRequired(false);
                _ = entity.Property(e => e.CreatedAt).IsRequired();
                _ = entity.Property(e => e.UpdatedAt).IsRequired();

                _ = entity.HasOne(e => e.Owner)
                    .WithMany()
                    .HasForeignKey(e => e.OwnerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            _ = modelBuilder.Entity<TaskItemEntity>(entity =>
            {
                _ = entity.HasKey(e => e.Id);
                _ = entity.Property(e => e.Title).IsRequired();
                _ = entity.Property(e => e.Description).IsRequired(false);
                _ = entity.Property(e => e.Status).IsRequired();
                _ = entity.Property(e => e.CreatedAt).IsRequired();
                _ = entity.Property(e => e.DueDate).IsRequired(false);

                _ = entity.HasOne(e => e.TodoList)
                    .WithMany(l => l.Tasks)
                    .HasForeignKey(e => e.TodoListId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            _ = modelBuilder.Entity<UserEntity>().HasData(
                new UserEntity
                {
                    Id = 1,
                    Username = "admin",
                    Email = "admin@example.com",
                    PasswordHash = "hashed-password"
                }
            );

            _ = modelBuilder.Entity<TodoListEntity>().HasData(
                new TodoListEntity
                {
                    Id = 1,
                    Title = "First List",
                    Description = "This is a seeded todo list",
                    CreatedAt = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 1, 2, 12, 0, 0, DateTimeKind.Utc),
                    OwnerId = 1
                }
            );

            _ = modelBuilder.Entity<TaskItemEntity>().HasData(
                new TaskItemEntity
                {
                    Id = 1,
                    Title = "First Task",
                    Description = "This is a seeded task",
                    Status = "Pending",
                    CreatedAt = new DateTime(2024, 1, 3, 9, 0, 0, DateTimeKind.Utc),
                    DueDate = new DateTime(2024, 1, 10, 9, 0, 0, DateTimeKind.Utc),
                    TodoListId = 1,
                }
            );
        }
    }
}
