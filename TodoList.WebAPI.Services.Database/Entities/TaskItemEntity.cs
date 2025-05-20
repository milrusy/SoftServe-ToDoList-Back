using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TodoList.WebAPI.Services.Database.Entities
{
    [Table("TaskItems")]
    public class TaskItemEntity
    {
        [Key]
        [Column("TaskItemId")]
        public int Id { get; set; }

        [Column("Title")]
        public string Title { get; set; } = default!;

        [Column("Description")]
        public string? Description { get; set; }

        [Column("Status")]
        public string Status { get; set; } = default!;

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("DueDate")]
        public DateTime? DueDate { get; set; }

        [ForeignKey(nameof(TodoList))]
        [Column("TodoListId")]
        public int TodoListId { get; set; }

        public virtual TodoListEntity TodoList { get; set; } = default!;
    }
}
