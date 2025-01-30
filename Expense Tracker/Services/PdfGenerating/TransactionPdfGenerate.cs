using Expense_Tracker.Contracts;
using Expense_Tracker.Models;
using Expense_Tracker.Services.Repositories;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Expense_Tracker.Services.PdfGenerating
{
    public class TransactionPdfGenerate : ITransactionPdfGenerate
    {
        ITransactionRepository _transactionRepository;
        public TransactionPdfGenerate(ITransactionRepository transactionRepository)
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
            _transactionRepository = transactionRepository;
        }

        public async Task<byte[]> TransactionGeneratePdf(DateTime? startDate, DateTime? endDate)
        {
            var transactions = (await _transactionRepository.GetAllTransactions()).OrderByDescending(date => date.Date).ToList();
            //var transactions = (await _transactionRepository.GetAllTransactions()).AsQueryable();

            if (startDate.HasValue && endDate.HasValue)
            {
                transactions = transactions
                    .Where(t => t.Date >= startDate.Value.AddDays(1) && t.Date <= endDate.Value.AddDays(1))
                    .OrderByDescending(date => date.Date).ToList();
            }
            else if (startDate.HasValue && !endDate.HasValue)
            {
                transactions = transactions
                    .Where(t => t.Date >= startDate.Value.AddDays(1))
                    .OrderByDescending(date => date.Date).ToList();
            }
            else if (endDate.HasValue && !startDate.HasValue)
            {
                transactions = transactions
                    .Where(t => t.Date <= endDate.Value.AddDays(1))
                    .OrderByDescending(date => date.Date).ToList();
            }
            else
            {
                transactions = transactions
                    .OrderByDescending(date => date.Date).ToList();
            }

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, QuestPDF.Infrastructure.Unit.Centimetre);

                    page.Header()
                        .Row(row =>
                        {
                            row.RelativeItem()
                                .Column(column =>
                                {
                                    column.Item()
                                        .Text("Expense Tracker")
                                        .FontFamily("Ubuntu")
                                        .FontSize(20)
                                        .Bold();

                                    column.Item().PaddingTop(1, QuestPDF.Infrastructure.Unit.Centimetre);
                                    if (startDate.HasValue && endDate.HasValue)
                                    {
                                        column.Item()
                                        .Text($"Report for: {startDate.GetValueOrDefault().AddDays(1).ToString("MMM-dd-yyy")} - " +
                                            $"{endDate.GetValueOrDefault().AddDays(1).ToString("MMM-dd-yyy")}")
                                        .FontFamily("Ubuntu")
                                        .FontSize(15);
                                    }                                    
                                });
                            row.RelativeItem()
                                .Column(column =>
                                {
                                    column.Item()
                                        .ShowOnce()
                                        .Text("TOTAL")
                                        .AlignRight()
                                        .FontFamily("Ubuntu")
                                        .ExtraBlack()
                                        .FontSize(27);
                                });

                        });

                    page.Content()
                        .Column(column =>
                        {
                            column.Item().PaddingTop(1, QuestPDF.Infrastructure.Unit.Centimetre);

                            column.Item().Table(async table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(40);//N#
                                    columns.RelativeColumn();//Category
                                    columns.RelativeColumn();//Note
                                    columns.RelativeColumn();//Amount
                                    columns.RelativeColumn();//Date

                                });
                                table.Header(header =>
                                {
                                    header.Cell().Padding(4).Text("№").Bold();
                                    header.Cell().Padding(4).Text("Category").Bold();
                                    header.Cell().Padding(4).Text("Note").Bold();
                                    header.Cell().Padding(4).Text("Amount").AlignRight().Bold();
                                    header.Cell().Padding(4).Text("Date").AlignRight().Bold();

                                    header.Cell().ColumnSpan(5).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                                });

                                for (var i = 0; i < transactions.Count; i++)
                                {
                                    var backgroundColor = i % 2 == 0 ?
                                        Color.FromHex("#ffffff") : Color.FromHex("#f0f0f0");

                                    var transactionItem = transactions[i];
                                    table.Cell().Padding(4).Text((i + 1).ToString());
                                    table.Cell().Padding(4).Text(transactionItem.Category.TitleWithIcon);
                                    table.Cell().Padding(4).Text(transactionItem.Note).Italic();
                                    if(transactionItem.Category.Type == "Income")
                                    {
                                        table.Cell().Padding(4).AlignRight().Text("+" + transactionItem.Amount.ToString("C2"));
                                    }
                                    else
                                    {
                                        table.Cell().Padding(4).AlignRight().Text("-" + transactionItem.Amount.ToString("C2"));
                                    }
                                    
                                    table.Cell().Padding(4).AlignRight().Text(transactionItem.Date.ToString("MMM-dd-yyy"));
                                }
                                table.Cell()
                                    .ColumnSpan(5)
                                    .PaddingVertical(5)
                                    .BorderBottom(1)
                                    .BorderColor(Colors.Black);
                                table.Cell().ColumnSpan(4).Text("Total Income").Bold().AlignRight();
                                table.Cell().AlignRight().Text(transactions.Where(i => i.Category.Type == "Income").Sum(i => i.Amount).ToString("C2")).FontColor(Color.FromHex("#65da41"));
                                table.Cell().ColumnSpan(4).Text("Total Expense").Bold().AlignRight();
                                table.Cell().AlignRight().Text(transactions.Where(i => i.Category.Type == "Expense").Sum(i => i.Amount).ToString("C2")).FontColor(Color.FromHex("#ff4e48"));
                                //table.Cell().ColumnSpan(4).Text($"Date: {DateTime.Today.ToString("dddd, dd MMMM yyyy")}").FontFamily("Ubuntu").FontSize(13).Bold().AlignLeft();

                                //column.Item().PaddingTop(1, QuestPDF.Infrastructure.Unit.Centimetre);
                                column.Item().Row(row =>
                                {
                                    row.RelativeItem()
                                        .Column(column2 =>
                                        {
                                            column2.Item()
                                                .Text($"Date: {DateTime.Today.ToString("dddd, dd MMMM yyyy")}")
                                                .AlignLeft()
                                                .FontFamily("Ubuntu")
                                                .FontSize(13)
                                                .Bold();
                                        });
                                });
                            });

                        });

                    page.Footer();
                });
            });
            return document.GeneratePdf();
        }
    }
}
//switch (startDate.HasValue && endDate.HasValue)
//{
//    case true:
//        var transactions = (await _transactionRepository.GetAllTransactions())
//            .Where(t => t.Date >= startDate && t.Date <= endDate)
//            .OrderByDescending(date => date.Date).ToList();
//        break;

//    case false:
//        var transactions = (await _transactionRepository.GetAllTransactions())
//            .OrderByDescending(date => date.Date).ToList();
//        break;

//    default:
//        if (startDate.HasValue && !endDate.HasValue)
//        {
//            var transactions = (await _transactionRepository.GetAllTransactions())
//                .Where(t => t.Date >= startDate)
//                .OrderByDescending(date => date.Date).ToList();
//        }
//        else if (endDate.HasValue && !startDate.HasValue)
//        {
//            var transactions = (await _transactionRepository.GetAllTransactions())
//                .Where(t => t.Date <= endDate)
//                .OrderByDescending(date => date.Date).ToList();
//        }
//        break;
//}