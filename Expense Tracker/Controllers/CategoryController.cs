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
using ValidationResult = FluentValidation.Results.ValidationResult;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Expense_Tracker.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IIconRepository _iconRepository;
        private IValidator<Category> _validator;

        public CategoryController(ICategoryRepository categoryRepository, IIconRepository iconRepository, IValidator<Category> validator)
        {
            _categoryRepository = categoryRepository;
            _iconRepository = iconRepository;
            _validator = validator;
        }

        // GET: Category
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAllCategories();
            return View(categories);
        }

        // GET: Category/AddOrEdit/5
        public async Task<IActionResult> AddOrEdit(Guid? id)
        {
            //PopulateIcons();
            var icons = await GetIconsForDropdown();
            ViewBag.Icons = icons;
            if (id == null)
            {
                ViewBag.Icons = icons;  //
                var category = new Category();
                return View(category);
            }

            else
            {
                var category = await _categoryRepository.GetCategory(id.Value);
                if (category == null)
                {
                    return NotFound();
                }
                ViewBag.Icons = icons;
                Console.WriteLine($"Editing Category - IconId: {category.IconId}");
                return View(category);
            }
        }

        // POST: Category/AddOrEdit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit( [Bind("Id,Title,IconId,Type")] Category category)
        {
            Console.WriteLine($"Received: Id={category.Id}, Title={category.Title}, IconId={category.IconId}, Type={category.Type}");

            ValidationResult validationResult = await _validator.ValidateAsync(category);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    Console.WriteLine($"Property: {error.PropertyName}, Error: {error.ErrorMessage}");
                }
            }
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(this.ModelState);
                await GetIconsForDropdown(); // Repopulate the dropdown
                return View(category);
            }
            if (ModelState.IsValid)
            {
                if (category.Id == Guid.Empty)
                {
                    ViewBag.Icons = await GetIconsForDropdown();//
                    await _categoryRepository.AddCategory(category);
                }
                else
                {
                    //await _categoryRepository.UpdateCategory(id, category);
                    await _categoryRepository.UpdateCategory(category.Id, category);

                    return RedirectToAction(nameof(Index));
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _categoryRepository.DeleteCategory(id);

            return RedirectToAction(nameof(Index));
        }
        [NonAction]
        public async Task PopulateIcons()
        {
            var icons = await _iconRepository.GetAllIcons();
            Icon defaultIcon = new Icon() { Id = 0, Name = "Choose Icon", Logo = "null"};
            icons.ToList().Insert(0, defaultIcon);
            ViewBag.Icons = icons;
        }
        private async Task<IEnumerable<SelectListItem>> GetIconsForDropdown()
        {
            var icons = await _iconRepository.GetAllIcons();
            return icons.Select(icon => new SelectListItem
            {
                Value = icon.Id.ToString(),
                Text = icon.Logo // Use Logo for dropdown display
            });
        }
    }
}
public static class Extensions
{
    public static void AddToModelState(this ValidationResult result, ModelStateDictionary modelState)
    {
        foreach (var error in result.Errors)
        {
            modelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }
    }
}
//private bool CategoryExists(Guid id)
//{
//    return _categoryRepository.Categories.Any(e => e.Id == id);
//}

// POST: Category/Create
// To protect from overposting attacks, enable the specific properties you want to bind to.
// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
//[HttpPost]
//[ValidateAntiForgeryToken]
//public async Task<IActionResult> AddOrEdit([Bind("Id,Title,IconId,Type")] Category category)
//{
//    if (ModelState.IsValid)
//    {
//        if(category.Id == null)
//        {
//            await _categoryRepository.AddCategory(category);
//        }
//        else
//        {
//            await _categoryRepository.UpdateCategory(Id, category);
//        }
//        return RedirectToAction(nameof(Index));
//    }

//    return View(category);
//}
// GET: Category/Delete/5
//public async Task<IActionResult> Delete(Guid? id)
//{
//    if (id == null)
//    {
//        return NotFound();
//    }

//    var category = await _categoryRepository.Categories
//        .Include(c => c.Icon)
//        .FirstOrDefaultAsync(m => m.Id == id);
//    if (category == null)
//    {
//        return NotFound();
//    }

//    return View(category);
//}

// GET: Category/Create
//public IActionResult Create()
//{
//    ViewData["IconId"] = new SelectList(_categoryRepository.Icons, "Id", "Name");
//    return View();
//}

