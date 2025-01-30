using Expense_Tracker.Services.Repositories;
using Microsoft.AspNetCore.Mvc;
using Expense_Tracker.Contracts;
using System.Drawing;
using System.Globalization;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Expense_Tracker.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardController(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<IActionResult> Index()
        {
            //Total Income
            var totalIncome = await _dashboardRepository.TotalIncome();
            ViewBag.TotalIncome = totalIncome.ToString("C2");

            //Total Expense
            var totalExpense = await _dashboardRepository.TotalExpense();
            ViewBag.TotalExpense = totalExpense.ToString("C2");

            //Open Balance
            var OpenBalance = totalIncome - totalExpense;
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            culture.NumberFormat.CurrencyNegativePattern = 1;
            ViewBag.OpenBalance = String.Format(culture, "{0:C2}", OpenBalance);

            //Doughnut Chart - Expense By Category
            ViewBag.DoughnutChart = await _dashboardRepository.DoughnutChart();

            //Bar Chart - Income&Expense By Date
            ViewBag.BarChart = await _dashboardRepository.BarChart();

            //GroupBar Chart - Income&Expense per Year
            ViewBag.GroupBarChart = await _dashboardRepository.GroupBarChart();

            return View();
        }

    }
}
//var Income7Days = await _dashboardRepository.IncomesLastSevenDays();
//ViewBag.Incomes7Days = Income7Days.ToString("C2");

//var Expense7Days = await _dashboardRepository.ExpensesLastSevenDays();
//ViewBag.Expenses7Days = Expense7Days.ToString("C2");

//var OpenBalance = Income7Days - Expense7Days;
//CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
//culture.NumberFormat.CurrencyNegativePattern = 1;
//ViewBag.OpenBalance = String.Format(culture, "{0:C2}", OpenBalance);

//public async Task<IActionResult> Index(string? type)
//{
//    string income, expense;

//    if (type == null || type == "Total")
//    {
//        var inc = await _dashboardRepository.TotalIncome();
//        income = inc.ToString("C2");
//        var exp = await _dashboardRepository.TotalExpense();
//        expense = exp.ToString("C2");
//    }
//    else
//    {
//        var inc = await _dashboardRepository.IncomesLastSevenDays();
//        income = inc.ToString("C2");
//        var exp = await _dashboardRepository.ExpensesLastSevenDays();
//        expense = exp.ToString("C2");
//    }

//    return Json(new { income, expense });
//}