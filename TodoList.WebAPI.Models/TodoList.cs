namespace TodoList.WebAPI.Models
{
    public class TodoList
    {
        public int Id { get; set; }

        public string Title { get; set; } = default!;

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int OwnerId { get; set; }

        public string? OwnerUsername { get; set; }

        public List<TaskItem>? Tasks { get; set; }
    }

}
