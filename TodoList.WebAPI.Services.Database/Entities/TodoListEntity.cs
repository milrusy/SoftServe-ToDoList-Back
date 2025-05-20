using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TodoList.WebAPI.Services.Database.Entities
{
    [Table("TodoLists")]
    public class TodoListEntity
    {
        [Key]
        [Column("TodoListId")]
        public int Id { get; set; }

        [Column("Title")]
        public string Title { get; set; } = default!;

        [Column("Description")]
        public string? Description { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(Owner))]
        [Column("OwnerId")]
        public int OwnerId { get; set; }

        public virtual UserEntity Owner { get; set; } = default!;

        public virtual ICollection<TaskItemEntity> Tasks { get; set; } = new List<TaskItemEntity>();
    }
}
