namespace Expense_Tracker.Models.DTOs
{
    public class ChartDto
    {
        public decimal Income { get; set; }
        public string IncomeFormatted { get; set; }
        public decimal Expense { get; set; }
        public string ExpenseFormatted { get; set; }
    }
}
