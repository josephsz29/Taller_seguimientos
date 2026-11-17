using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NoteManager.Models
{
    // Página de categorías: formulario para agregar + lista con la cantidad de notas
    public class CategoryIndexViewModel
    {
        [Required(ErrorMessage = "Escribe el nombre de la categoría")]
        [StringLength(50, ErrorMessage = "El nombre no puede superar 50 caracteres")]
        [Display(Name = "Nueva categoría")]
        public string Name { get; set; } = string.Empty;

        [BindNever]
        public List<CategorySummary> Categories { get; set; } = new();
    }
}
