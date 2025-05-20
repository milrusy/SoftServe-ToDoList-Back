namespace TodoList.WebAPI.Models.DTOs
{
    public class TodoListDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int OwnerId { get; set; } = default!;
    }

}
