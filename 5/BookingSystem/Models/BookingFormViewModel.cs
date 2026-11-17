using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BookingSystem.Models
{
    // Datos del formulario para reservar un horario
    public class BookingFormViewModel
    {
        public int ServiceId { get; set; }
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "Escribe tu nombre")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar 100 caracteres")]
        [Display(Name = "Nombre completo")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Escribe tu correo electrónico")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido")]
        [StringLength(150, ErrorMessage = "El correo no puede superar 150 caracteres")]
        [Display(Name = "Correo electrónico")]
        public string CustomerEmail { get; set; } = string.Empty;

        [Phone(ErrorMessage = "El teléfono no es válido")]
        [StringLength(30, ErrorMessage = "El teléfono no puede superar 30 caracteres")]
        [Display(Name = "Teléfono (opcional)")]
        public string? CustomerPhone { get; set; }

        // Solo para mostrar el resumen; no viene del formulario
        [BindNever]
        public BookableService? Service { get; set; }

        public DateTime EndTime => StartTime.AddMinutes(Service?.DurationMinutes ?? 0);

        public string StartValue => StartTime.ToString("yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
        public string DateValue => StartTime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        public string DateLabel => StartTime.ToString("dddd d 'de' MMMM 'de' yyyy", new CultureInfo("es-CO"));

        public string TimeLabel =>
            StartTime.ToString("HH:mm", CultureInfo.InvariantCulture) + " - " +
            EndTime.ToString("HH:mm", CultureInfo.InvariantCulture);
    }
}
