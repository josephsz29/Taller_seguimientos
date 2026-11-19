using System.ComponentModel.DataAnnotations;

namespace SurveyPlatform.Models
{
    // Datos del formulario para crear o editar una encuesta (Id = 0 significa encuesta nueva)
    public class SurveyFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Escribe el título de la encuesta")]
        [StringLength(150, ErrorMessage = "El título no puede superar 150 caracteres")]
        [Display(Name = "Título")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede superar 500 caracteres")]
        [Display(Name = "Descripción (opcional)")]
        public string? Description { get; set; }
    }
}
