using System.Globalization;

namespace BookingSystem.Models
{
    // Un horario del día, con su disponibilidad (no se guarda en la base de datos)
    public class TimeSlot
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsAvailable { get; set; }

        // Formato "yyyy-MM-ddTHH:mm" para enviarlo por la URL
        public string StartValue => StartTime.ToString("yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);

        // Texto que se muestra en el botón, por ejemplo "09:30"
        public string Label => StartTime.ToString("HH:mm", CultureInfo.InvariantCulture);
    }
}
