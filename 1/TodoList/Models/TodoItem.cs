using System.ComponentModel.DataAnnotations;

namespace TodoList.Models
{
    public class TodoItem
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public bool IsCompleted { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
