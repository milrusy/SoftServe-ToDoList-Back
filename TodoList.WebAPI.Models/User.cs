namespace TodoList.WebAPI.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; } = default!;

        public string Email { get; set; } = default!;

        public ICollection<TodoList>? TodoLists { get; set; }
    }
}
