using ExpenseTracker.Models;

namespace ExpenseTracker.Services
{
    public interface IExpenseService
    {
        Task<List<Expense>> GetByMonthAsync(int year, int month);
        Task AddAsync(Expense expense);
        Task DeleteAsync(int id);
        Task<List<Category>> GetCategoriesAsync();
        Task<MonthlySummary> GetMonthlySummaryAsync(int year, int month);
    }
}
