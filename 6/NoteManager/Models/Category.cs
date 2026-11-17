using System.ComponentModel.DataAnnotations;

namespace NoteManager.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        // Una categoría tiene muchas notas
        public List<Note> Notes { get; set; } = new();
    }
}
