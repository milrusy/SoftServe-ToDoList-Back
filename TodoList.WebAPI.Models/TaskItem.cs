namespace TodoList.WebAPI.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        public string Title { get; set; } = default!;

        public string? Description { get; set; }

        public TaskStatus Status { get; set; } = default!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? DueDate { get; set; }

        public int TodoListId { get; set; } = default!;
    }
}
