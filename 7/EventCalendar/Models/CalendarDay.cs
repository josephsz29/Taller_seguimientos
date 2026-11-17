using System.Globalization;

namespace EventCalendar.Models
{
    // Un día de la cuadrícula del calendario (no se guarda en la base de datos)
    public class CalendarDay
    {
        public DateTime Date { get; set; }
        public bool IsCurrentMonth { get; set; }
        public bool IsToday { get; set; }
        public List<CalendarEvent> Events { get; set; } = new();

        // Formato "yyyy-MM-dd" para enviarlo por la URL
        public string DateValue => Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    }
}
