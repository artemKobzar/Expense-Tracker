using Expense_Tracker.Services.PdfGenerating;
using Microsoft.AspNetCore.Mvc;

namespace Expense_Tracker.Controllers
{
    public class ReportController : Controller
    {
        private readonly ITransactionPdfGenerate _transactionPdfGenerate;

        public ReportController(ITransactionPdfGenerate transactionPdfGenerate)
        {
            _transactionPdfGenerate = transactionPdfGenerate;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> TransactionGeneratePdf(DateTime? startDate, DateTime? endDate)
        {
            var file = await _transactionPdfGenerate.TransactionGeneratePdf(startDate, endDate);

            return File(file, "application/pdf", "TransactionsReport.pdf");
        }
    }
}
