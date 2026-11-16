using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        // Una categoría tiene muchos gastos
        public List<Expense> Expenses { get; set; } = new();
    }
}
