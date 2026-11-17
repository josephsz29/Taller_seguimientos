using System.ComponentModel.DataAnnotations;

namespace EventCalendar.Models
{
    public class CalendarEvent
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        // Minutos antes del inicio en que se avisa (null = sin recordatorio)
        public int? ReminderMinutes { get; set; }

        // Se pone en true cuando el usuario cierra el aviso, para que no vuelva a aparecer
        public bool ReminderDismissed { get; set; }

        // Momento exacto del aviso. Es de solo lectura, así que EF no la guarda en la base de datos.
        public DateTime? ReminderTime =>
            ReminderMinutes.HasValue ? StartTime.AddMinutes(-ReminderMinutes.Value) : null;
    }
}
