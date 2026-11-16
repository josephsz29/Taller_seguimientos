namespace ExpenseTracker.Models
{
    public class CategorySummary
    {
        public string CategoryName { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public decimal Percentage { get; set; }
    }
}
