using System.Globalization;

namespace BookingSystem.Models
{
    // Datos de la página de horarios disponibles
    public class SlotsViewModel
    {
        public BookableService Service { get; set; } = null!;
        public DateTime Date { get; set; }
        public List<TimeSlot> Slots { get; set; } = new();

        public bool IsBusinessDay => BookingRules.IsBusinessDay(Date);

        // Formato "yyyy-MM-dd" que usa el <input type="date">
        public string DateValue => Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        public string MinDateValue => DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        // Texto en español, por ejemplo "martes 17 de noviembre de 2026"
        public string DateLabel => Date.ToString("dddd d 'de' MMMM 'de' yyyy", new CultureInfo("es-CO"));
    }
}
