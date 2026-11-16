using Microsoft.EntityFrameworkCore;
using TipCalculator.Data;
using TipCalculator.Models;

namespace TipCalculator.Services
{
    public class TipService : ITipService
    {
        private readonly AppDbContext _context;

        public TipService(AppDbContext context)
        {
            _context = context;
        }

        public TipResult Calculate(decimal billAmount, decimal tipPercentage)
        {
            // propina = monto * porcentaje / 100 (decimal para evitar errores de redondeo con dinero)
            var tipAmount = Math.Round(billAmount * tipPercentage / 100m, 2);

            return new TipResult
            {
                TipAmount = tipAmount,
                TotalWithTip = billAmount + tipAmount
            };
        }

        public async Task<TipResult> CalculateAndSaveAsync(decimal billAmount, decimal tipPercentage)
        {
            var result = Calculate(billAmount, tipPercentage);

            // Guardamos el cálculo en el historial
            _context.TipRecords.Add(new TipRecord
            {
                BillAmount = billAmount,
                TipPercentage = tipPercentage,
                TipAmount = result.TipAmount,
                TotalWithTip = result.TotalWithTip
            });
            await _context.SaveChangesAsync();

            return result;
        }

        public async Task<List<TipRecord>> GetHistoryAsync(int count = 10)
        {
            return await _context.TipRecords
                .OrderByDescending(r => r.CreatedAt)
                .Take(count)
                .ToListAsync();
        }
    }
}
