using TipCalculator.Models;

namespace TipCalculator.Services
{
    public interface ITipService
    {
        TipResult Calculate(decimal billAmount, decimal tipPercentage);
        Task<TipResult> CalculateAndSaveAsync(decimal billAmount, decimal tipPercentage);
        Task<List<TipRecord>> GetHistoryAsync(int count = 10);
    }
}
