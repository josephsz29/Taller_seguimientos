using System.ComponentModel.DataAnnotations;

namespace NoteManager.Models
{
    public class Note
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        // Sin [StringLength]: en SQL Server se guarda como nvarchar(max)
        [Required]
        public string Content { get; set; } = string.Empty;

        // Llave foránea y propiedad de navegación hacia la categoría
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
