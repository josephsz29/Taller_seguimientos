using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NoteManager.Models
{
    // Datos del formulario para crear o editar una nota (Id = 0 significa nota nueva)
    public class NoteFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Escribe un título")]
        [StringLength(150, ErrorMessage = "El título no puede superar 150 caracteres")]
        [Display(Name = "Título")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Escribe el contenido de la nota")]
        [Display(Name = "Contenido")]
        public string Content { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Selecciona una categoría")]
        [Display(Name = "Categoría")]
        public int CategoryId { get; set; }

        // Opciones del <select>; no vienen del formulario
        [BindNever]
        public List<SelectListItem> Categories { get; set; } = new();
    }
}
