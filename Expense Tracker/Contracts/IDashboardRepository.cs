using Expense_Tracker.Models;
using Expense_Tracker.Models.DTOs;

namespace Expense_Tracker.Contracts
{
    public interface IDashboardRepository
    {
        Task<IEnumerable<BarChartDto>> BarChart();
        Task<decimal> CustomExpense(string? period);
        Task<IEnumerable<DoughnutChartDto>> DoughnutChart();
        Task<decimal> ExpensesLastSevenDays();
        Task<IEnumerable<GroupBarChartDto>> GroupBarChart();
        Task<decimal> IncomesLastSevenDays();
        Task<decimal> TotalExpense();
        Task<decimal> TotalIncome();
        Task<IEnumerable<Transaction>> TransactionsLastSevenDays();
    }
}