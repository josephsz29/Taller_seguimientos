using System.Globalization;

namespace EventCalendar.Models
{
    // Un mes completo: 42 días (6 semanas) para que la cuadrícula siempre tenga el mismo tamaño
    public class CalendarMonth
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public List<CalendarDay> Days { get; set; } = new();

        private DateTime FirstDay => new DateTime(Year, Month, 1);

        // Texto en español, por ejemplo "noviembre 2026"
        public string Label => FirstDay.ToString("MMMM yyyy", new CultureInfo("es-CO"));

        public string MonthValue => FirstDay.ToString("yyyy-MM", CultureInfo.InvariantCulture);
        public string PreviousMonthValue => FirstDay.AddMonths(-1).ToString("yyyy-MM", CultureInfo.InvariantCulture);
        public string NextMonthValue => FirstDay.AddMonths(1).ToString("yyyy-MM", CultureInfo.InvariantCulture);
    }
}
