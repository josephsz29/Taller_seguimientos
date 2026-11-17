using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace EventCalendar.Models
{
    // Datos del formulario para crear o editar un evento (Id = 0 significa evento nuevo)
    public class EventFormViewModel : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Escribe un título")]
        [StringLength(100, ErrorMessage = "El título no puede superar 100 caracteres")]
        [Display(Name = "Título")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede superar 500 caracteres")]
        [Display(Name = "Descripción (opcional)")]
        public string? Description { get; set; }

        [Display(Name = "Inicio")]
        public DateTime StartTime { get; set; }

        [Display(Name = "Fin")]
        public DateTime EndTime { get; set; }

        [Range(0, 10080, ErrorMessage = "El recordatorio no es válido")]
        [Display(Name = "Recordatorio")]
        public int? ReminderMinutes { get; set; }

        // Formato "yyyy-MM-ddTHH:mm" que usa el <input type="datetime-local">
        public string StartValue => StartTime.ToString("yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
        public string EndValue => EndTime.ToString("yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);

        // Mes del evento, para volver a ese mes del calendario
        public string MonthValue => StartTime.ToString("yyyy-MM", CultureInfo.InvariantCulture);

        // Validación entre dos campos: el fin debe ser posterior al inicio
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndTime <= StartTime)
            {
                yield return new ValidationResult(
                    "La hora de fin debe ser posterior a la de inicio.",
                    new[] { nameof(EndTime) });
            }
        }
    }
}
