using System.Globalization;
using ExpenseTracker.Models;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpenseTracker.Controllers
{
    public class ExpensesController : Controller
    {
        private readonly IExpenseService _expenseService;

        public ExpensesController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        // GET: /Expenses?month=2026-11  -> formulario + lista de gastos del mes
        [HttpGet]
        public async Task<IActionResult> Index(string? month)
        {
            var selectedMonth = ParseMonth(month);
            var model = await BuildIndexViewModelAsync(selectedMonth, new ExpenseFormViewModel());
            return View(model);
        }

        // POST: /Expenses/Create  -> valida y guarda el gasto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(Prefix = "Form")] ExpenseFormViewModel form, string? month)
        {
            if (!ModelState.IsValid)
            {
                // Volvemos a mostrar la página con los errores y lo que el usuario escribió
                var model = await BuildIndexViewModelAsync(ParseMonth(month), form);
                return View(nameof(Index), model);
            }

            await _expenseService.AddAsync(new Expense
            {
                Description = form.Description.Trim(),
                Amount = form.Amount!.Value,
                Date = form.Date.Date,
                CategoryId = form.CategoryId
            });

            // Mostramos el mes del gasto recién registrado
            return RedirectToAction(nameof(Index), new { month = form.Date.ToString("yyyy-MM", CultureInfo.InvariantCulture) });
        }

        // POST: /Expenses/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string? month)
        {
            await _expenseService.DeleteAsync(id);
            return RedirectToAction(nameof(Index), new { month });
        }

        // GET: /Expenses/Summary?month=2026-11  -> resumen mensual por categoría
        [HttpGet]
        public async Task<IActionResult> Summary(string? month)
        {
            var selectedMonth = ParseMonth(month);
            var summary = await _expenseService.GetMonthlySummaryAsync(selectedMonth.Year, selectedMonth.Month);
            return View(summary);
        }

        public IActionResult Error()
        {
            return Content("Ocurrió un error inesperado.");
        }

        private async Task<ExpenseIndexViewModel> BuildIndexViewModelAsync(DateTime selectedMonth, ExpenseFormViewModel form)
        {
            var expenses = await _expenseService.GetByMonthAsync(selectedMonth.Year, selectedMonth.Month);
            var categories = await _expenseService.GetCategoriesAsync();

            return new ExpenseIndexViewModel
            {
                SelectedMonth = selectedMonth,
                Form = form,
                Expenses = expenses,
                MonthTotal = expenses.Sum(e => e.Amount),
                Categories = categories
                    .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
                    .ToList()
            };
        }

        // Convierte "2026-11" en el primer día de ese mes; si no es válido, usa el mes actual
        private static DateTime ParseMonth(string? month)
        {
            if (!string.IsNullOrEmpty(month) &&
                DateTime.TryParseExact(month + "-01", "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var parsed))
            {
                return parsed;
            }

            var today = DateTime.Today;
            return new DateTime(today.Year, today.Month, 1);
        }
    }
}
