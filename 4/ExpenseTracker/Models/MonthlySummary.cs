using System.Globalization;

namespace ExpenseTracker.Models
{
    public class MonthlySummary
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal TotalAmount { get; set; }
        public int ExpenseCount { get; set; }
        public List<CategorySummary> Categories { get; set; } = new();

        // Formato "yyyy-MM" que usa el <input type="month">
        public string MonthValue => $"{Year:D4}-{Month:D2}";

        // Texto en español, por ejemplo "noviembre 2026"
        public string MonthLabel => new DateTime(Year, Month, 1).ToString("MMMM yyyy", new CultureInfo("es-CO"));
    }
}
