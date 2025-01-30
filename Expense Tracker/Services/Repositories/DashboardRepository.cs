using Expense_Tracker.Contracts;
using Expense_Tracker.Data;
using Expense_Tracker.Models;
using Expense_Tracker.Models.DTOs;
using System.Globalization;
using System.Linq;

namespace Expense_Tracker.Services.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly ExpenseTrackerDbContext _dbContext;
        ITransactionRepository _transactionRepository;
        DateTime StartDate;
        DateTime EndDate = DateTime.Today;

        public DashboardRepository(ExpenseTrackerDbContext dbContext, ITransactionRepository transactionRepository)
        {
            _dbContext = dbContext;
            _transactionRepository = transactionRepository;
        }

        public async Task<decimal> TotalIncome()
        {
            var transactionAll = await _transactionRepository.GetAllTransactions();
            var totalIncome = transactionAll.Where(i => i.Category.Type == "Income").Sum(i => i.Amount);
            return totalIncome;
        }
        public async Task<decimal> TotalExpense()
        {
            var transactionAll = await _transactionRepository.GetAllTransactions();
            var totalExpense = transactionAll.Where(i => i.Category.Type == "Expense").Sum(i => i.Amount);
            return totalExpense;
        }
        public async Task<IEnumerable<DoughnutChartDto>> DoughnutChart()
        {
            var transactions = await _transactionRepository.GetAllTransactions();
            List<DoughnutChartDto> SelectedTransactions = transactions
                .Where(i => i.Category.Type == "Expense")
                .GroupBy(j => j.Category.Id)
                .Select(t => new DoughnutChartDto
                {
                    TitleWithIcon = t.First().Category.TitleWithIcon,
                    Amount = t.Sum(j => j.Amount),
                    AmountFormatted = t.Sum(j => j.Amount).ToString("C2")
                })
                .OrderByDescending(l => l.Amount)
                .ToList();

            return SelectedTransactions;
        }
        public async Task<IEnumerable<GroupBarChartDto>> GroupBarChart()
        {
            DateTime startDate = new DateTime(2024, 1, 1);
            DateTime endDate = new DateTime(2024, 12, 31); 
            var transactions = await _transactionRepository.GetAllTransactions();

            List<Transaction> SelectedTransactions = transactions
                .Where(t => t.Date >= startDate && t.Date <= endDate)
                .ToList();

            //Income Transaction per Year
            var IncomeTransactions = SelectedTransactions
                .Where(t => t.Category.Type == "Income")
                .GroupBy(t => t.Date.Month)
                .Select(k => new 
                {
                    Month = k.Key,
                    Income = k.Sum(l => l.Amount)
                })
                .ToDictionary(x => x.Month, x => x.Income);

            //Expense Transaction per Year
            var ExpenseTransactions = SelectedTransactions
                .Where(t => t.Category.Type == "Expense")
                .GroupBy(t => t.Date.Month)
                .Select(k => new 
                {
                    Month = k.Key,
                    Expense = k.Sum(l => l.Amount)
                })
                .ToDictionary(x => x.Month, x => x.Expense);

            // Combine Income & Expense
            var groupBarChartData = Enumerable.Range(1, 12)
                .Select(month => new GroupBarChartDto
                {
                    Month = new DateTime(2024, month, 1).ToString("MMM"),
                    Income = IncomeTransactions.ContainsKey(month) ? IncomeTransactions[month] : 0,
                    IncomeFormatted = IncomeTransactions.ContainsKey(month) ? IncomeTransactions[month].ToString("C2") : "$0.00",
                    Expense = ExpenseTransactions.ContainsKey(month) ? ExpenseTransactions[month] : 0,
                    ExpenseFormatted = ExpenseTransactions.ContainsKey(month) ? ExpenseTransactions[month].ToString("C2") : "$0.00"
                }).ToList();

            return groupBarChartData;
        }
        public async Task<IEnumerable<BarChartDto>> BarChart()
        {
            DateTime startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);
            var transactions = await _transactionRepository.GetAllTransactions();

            List<Transaction> SelectedTransactions = transactions
                .Where(t => t.Date >= startDate && t.Date <= endDate)
                .ToList();

            //Income Transaction for current month
            List<BarChartDto> IncomeTransactions = SelectedTransactions
                .Where(t => t.Category.Type == "Income")
                .GroupBy(d => d.Date)
                .Select(k => new BarChartDto
                {
                    Day = k.First().Date.ToString("dd-MMM"),
                    Income = k.Sum(l => l.Amount),
                    IncomeFormatted = k.Sum(l => l.Amount).ToString("C2")
                })
                .ToList();

            //Expense Transaction for current month
            List<BarChartDto> ExpenseTransactions = SelectedTransactions
                .Where(t => t.Category.Type == "Expense")
                .GroupBy(d => d.Date)
                .Select(k => new BarChartDto
                {
                    Day = k.First().Date.ToString("dd-MMM"),
                    Expense = k.Sum(l => l.Amount),
                    ExpenseFormatted = k.Sum(l => l.Amount).ToString("C2")
                })
                .ToList();

            // Days of the current month
            int daysInMonth = DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month);
            string[] CurrentMonthDays = Enumerable.Range(1, daysInMonth)
                .Select(i => startDate.AddDays(i - 1).ToString("dd-MMM"))
                .ToArray();

            // Combine Income & Expense
            var BarChartData = CurrentMonthDays
                .GroupJoin(IncomeTransactions,
                    day => day,
                    income => income.Day,
                    (day, incomeGroup) => new { day, income = incomeGroup.FirstOrDefault() })
                .GroupJoin(ExpenseTransactions,
                    dayWithIncome => dayWithIncome.day,
                    expense => expense.Day,
                    (dayWithIcome, expenseGroup) => new BarChartDto
                    {
                        Day = dayWithIcome.day,
                        Income = dayWithIcome.income?.Income ?? 0,
                        IncomeFormatted = (dayWithIcome.income?.Income ?? 0).ToString("C2"),
                        Expense = expenseGroup.FirstOrDefault()?.Expense ?? 0,
                        ExpenseFormatted = (expenseGroup.FirstOrDefault()?.Expense ?? 0).ToString("C2")
                    })
                .ToList();

            return BarChartData;
        }
        public async Task<IEnumerable<Transaction>> TransactionsLastSevenDays()
        {
            var transactionAll = await _transactionRepository.GetAllTransactions();
            StartDate = DateTime.Today.AddDays(-6);
            //7 Days Transactions
            List<Transaction> transaction7Days = transactionAll.Where(t => t.Date >= StartDate && t.Date <= EndDate).ToList();
            return transaction7Days;

        }
        public async Task<decimal> IncomesLastSevenDays()
        {
            var transactions7Days = await TransactionsLastSevenDays();

            //7 Days Income
            List<Transaction> incomes7Days = transactions7Days.ToList();
            var filteredIncomes = incomes7Days.Where(i => i.Category.Type == "Income")
            .Sum(j => j.Amount);
            return filteredIncomes;
        }
        public async Task<decimal> ExpensesLastSevenDays()
        {
            var transactions7Days = await TransactionsLastSevenDays();

            //7 Days Expense
            List<Transaction> expenses7Days = transactions7Days.ToList();
            var filteredExpenses = expenses7Days.Where(i => i.Category.Type == "Expense")
            .Sum(j => j.Amount);
            return filteredExpenses;
        }
        public async Task<decimal> CustomExpense(string? period)
        {
            if(period == "Seven Days")
            {
                var result = await ExpensesLastSevenDays();
                return result;
            }
            else
            {
                var result = await TotalExpense();
                return result;
            }
        }
    }
}
//var GroupBarChartData = CurrentMonths
//    .GroupJoin(IncomeTransactions,
//        month => month,
//        income => income.Month,
//        (month, incomeGroup) => new { month, income = incomeGroup.FirstOrDefault() })
//    .GroupJoin(ExpenseTransactions,
//        monthWithIncome => monthWithIncome.month,
//        expense => expense.Month,
//        (monthWithIcome, expenseGroup) => new GroupBarChartDto
//        {
//            Month = monthWithIcome.month,
//            Income = monthWithIcome.income?.Income ?? 0,
//            IncomeFormatted = (monthWithIcome.income?.Income ?? 0).ToString("C2"),
//            Expense = expenseGroup.FirstOrDefault()?.Expense ?? 0,
//            ExpenseFormatted = (expenseGroup.FirstOrDefault()?.Expense ?? 0).ToString("C2")
//        })
//    .ToList();