using ExpenseTracker.Data;
using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly AppDbContext _context;

        public ExpenseService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Expense>> GetByMonthAsync(int year, int month)
        {
            var start = new DateTime(year, month, 1);
            var end = start.AddMonths(1);

            // Rango [inicio del mes, inicio del mes siguiente)
            return await _context.Expenses
                .Include(e => e.Category)
                .Where(e => e.Date >= start && e.Date < end)
                .OrderByDescending(e => e.Date)
                .ThenByDescending(e => e.Id)
                .ToListAsync();
        }

        public async Task AddAsync(Expense expense)
        {
            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense != null)
            {
                _context.Expenses.Remove(expense);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _context.Categories
                .OrderBy(c => c.Id)
                .ToListAsync();
        }

        public async Task<MonthlySummary> GetMonthlySummaryAsync(int year, int month)
        {
            var start = new DateTime(year, month, 1);
            var end = start.AddMonths(1);

            var monthlyExpenses = _context.Expenses
                .Where(e => e.Date >= start && e.Date < end);

            // La base de datos agrupa y suma por categoría
            var totalsByCategory = await monthlyExpenses
                .GroupBy(e => e.Category.Name)
                .Select(g => new { Name = g.Key, Total = g.Sum(e => e.Amount) })
                .ToListAsync();

            var expenseCount = await monthlyExpenses.CountAsync();
            var totalAmount = totalsByCategory.Sum(t => t.Total);

            return new MonthlySummary
            {
                Year = year,
                Month = month,
                TotalAmount = totalAmount,
                ExpenseCount = expenseCount,
                Categories = totalsByCategory
                    .OrderByDescending(t => t.Total)
                    .Select(t => new CategorySummary
                    {
                        CategoryName = t.Name,
                        Total = t.Total,
                        Percentage = totalAmount > 0
                            ? Math.Round(t.Total / totalAmount * 100m, 1)
                            : 0m
                    })
                    .ToList()
            };
        }
    }
}
