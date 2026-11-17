using Microsoft.AspNetCore.Mvc.Rendering;

namespace NoteManager.Models
{
    // Datos de la página principal: búsqueda, filtro por categoría y lista de notas
    public class NoteIndexViewModel
    {
        public string? Search { get; set; }
        public int? CategoryId { get; set; }
        public List<Note> Notes { get; set; } = new();
        public List<SelectListItem> Categories { get; set; } = new();
    }
}
