namespace Expense_Tracker.Services.PdfGenerating
{
    public interface ITransactionPdfGenerate
    {
        Task<byte[]> TransactionGeneratePdf(DateTime? startDate, DateTime? endDate);
    }
}