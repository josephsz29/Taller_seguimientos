namespace EventCalendar.Models
{
    // Datos de la página principal: calendario del mes + próximos eventos
    public class CalendarIndexViewModel
    {
        public CalendarMonth Month { get; set; } = new();
        public List<CalendarEvent> Upcoming { get; set; } = new();
    }
}
