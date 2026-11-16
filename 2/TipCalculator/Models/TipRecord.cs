using Microsoft.EntityFrameworkCore;

namespace TipCalculator.Models
{
    public class TipRecord
    {
        public int Id { get; set; }

        [Precision(18, 2)]
        public decimal BillAmount { get; set; }

        [Precision(5, 2)]
        public decimal TipPercentage { get; set; }

        [Precision(18, 2)]
        public decimal TipAmount { get; set; }

        [Precision(18, 2)]
        public decimal TotalWithTip { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
