using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TodoList.WebAPI.Services.Database.Entities
{
    [Table("Users")]
    public class UserEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("UserId")]
        public int Id { get; set; }

        [Column("Username")]
        public string Username { get; set; } = default!;

        [Column("Email")]
        public string Email { get; set; } = default!;

        [Column("PasswordHash")]
        public string PasswordHash { get; set; } = default!;
        public virtual ICollection<TodoListEntity> TodoLists { get; set; } = new List<TodoListEntity>();
    }
}
