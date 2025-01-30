using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Expense_Tracker.Data;
using Expense_Tracker.Models;
using Expense_Tracker.Contracts;
using FluentValidation;
using FluentValidation.Results;

namespace Expense_Tracker.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IIconRepository _iconRepository;
        private readonly ITransactionRepository _transactionRepository;
        private IValidator<Transaction> _validator;

        public TransactionController(ICategoryRepository categoryRepository, IIconRepository iconRepository,
            ITransactionRepository transactionRepository, IValidator<Transaction> validator)
        {
            _categoryRepository = categoryRepository;
            _iconRepository = iconRepository;
            _transactionRepository = transactionRepository;
            _validator = validator;
        }

        // GET: Transaction
        public async Task<IActionResult> Index()
        {
            var transaction = await _transactionRepository.GetAllTransactions();
            return View(transaction);
        }

        // GET: Transaction/AddOrEdit
        public async Task<IActionResult> AddOrEdit(Guid? id)
        {
            var categories = await GetCategoriesForDropdown();
            ViewBag.Categories = categories;
            if (id == null)
            {
                ViewBag.Categories = categories;  //
                var transaction = new Transaction();
                return View(transaction);
            }

            else
            {
                var transaction = await _transactionRepository.GetTransaction(id.Value);
                if (transaction == null)
                {
                    return NotFound();
                }
                ViewBag.Categories = categories;
                Console.WriteLine($"Editing Category - IconId: {transaction.CategoryId}");
                return View(transaction);
            }
        }

        // POST: Transaction/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("Id,CategoryId,Amount,Note,Date")] Transaction transaction)
        {
            Console.WriteLine($"Received: Id={transaction.Id}, IconId={transaction.CategoryId}");

            var categories = await GetCategoriesForDropdown();
            ViewBag.Categories = categories;

            ValidationResult validationResult = await _validator.ValidateAsync(transaction);

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(this.ModelState);
                ViewBag.Categories = categories; // Repopulate the dropdown
                return View(transaction);
            }
            //ViewBag.Categories = categories;

            if (ModelState.IsValid)
            {
                if (transaction.Id == Guid.Empty)
                {
                    ViewBag.Categories = categories;//
                    await _transactionRepository.AddTransaction(transaction);
                }
                else
                {
                    //await _categoryRepository.UpdateCategory(id, category);
                    await _transactionRepository.UpdateTransaction(transaction.Id, transaction);

                    return RedirectToAction(nameof(Index));
                }
            }
            return RedirectToAction(nameof(Index));
        }


        // POST: Transaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _transactionRepository.DeleteTransaction(id);

            return RedirectToAction(nameof(Index));
        }
        private async Task<IEnumerable<SelectListItem>> GetCategoriesForDropdown()
        {
            var categories = await _categoryRepository.GetAllCategories();
            var categoriesWithDefaultCategory = new List<Category>()
            {
                new Category { Id = Guid.Empty, Title = "Choose a Category" }
            };
            categoriesWithDefaultCategory.AddRange(categories);
            return categoriesWithDefaultCategory.Select(category => new SelectListItem
            {
                Value = category.Id.ToString(),
                Text = category.TitleWithIcon // Use Logo for dropdown display
            });
        }
    }
}
//if (!ModelState.IsValid)
//{
//    await GetCategoriesForDropdown(); // Repopulate the dropdown
//    return View(transaction);
//}
