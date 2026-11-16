using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpenseTracker.Models
{
    // Datos de la página principal: formulario + lista de gastos del mes
    public class ExpenseIndexViewModel
    {
        // Primer día del mes que se está viendo
        public DateTime SelectedMonth { get; set; }

        // Formato "yyyy-MM" que usa el <input type="month">
        public string MonthValue => SelectedMonth.ToString("yyyy-MM", CultureInfo.InvariantCulture);

        // Texto en español, por ejemplo "noviembre 2026"
        public string MonthLabel => SelectedMonth.ToString("MMMM yyyy", new CultureInfo("es-CO"));

        public ExpenseFormViewModel Form { get; set; } = new();
        public List<Expense> Expenses { get; set; } = new();
        public decimal MonthTotal { get; set; }
        public List<SelectListItem> Categories { get; set; } = new();
    }
}
