using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Models
{
    public class Expense
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;

        [Precision(18, 2)]
        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        // Llave foránea y propiedad de navegación hacia la categoría
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}
