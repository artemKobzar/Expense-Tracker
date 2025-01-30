using Expense_Tracker.Contracts;
using Expense_Tracker.Data;
using Expense_Tracker.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Expense_Tracker.Services.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ExpenseTrackerDbContext _dbContext;
        public CategoryRepository(ExpenseTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            var categories = await _dbContext.Categories.Include(i => i.Icon).ToListAsync();
            return categories;
        }
        public async Task<Category> GetCategory(Guid id)
        {
            var category = await _dbContext.Categories.Include(c => c.Icon).FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                throw new KeyNotFoundException("Category not found");
            }

            return category;
        }
        public async Task AddCategory(Category category)
        {
            var icon = await _dbContext.Icons.FindAsync(category.IconId);
            if(icon != null)
            {
                category.Icon = icon;
            }
            else
            {
                // Handle case when Icon is not found
                throw new InvalidOperationException("Icon not found.");
            }
            _dbContext.Attach(category.Icon);
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();
        }
        public async Task UpdateCategory(Guid id, Category updatedCategory)
        {
            if (updatedCategory == null)
            {
                throw new ArgumentNullException(nameof(updatedCategory));
            }
            var category = await _dbContext.Categories.FindAsync(id);

            if (category == null)
            {
                throw new KeyNotFoundException("Category not found");
            }
            category.Title = updatedCategory.Title;
            category.IconId = updatedCategory.IconId;
            category.Type = updatedCategory.Type;
            var icon = await _dbContext.Icons.FindAsync(updatedCategory.IconId);
            if (icon != null)
            {
                category.Icon = icon;
            }
            else
            {
                // Handle case when Icon is not found
                throw new InvalidOperationException("Icon not found.");
            }

            await _dbContext.SaveChangesAsync();
        }
        public async Task DeleteCategory(Guid id)
        {
            var category = await _dbContext.Categories.FindAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException("Category not found");
            }
            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();
        }
    }
}
