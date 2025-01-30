using Expense_Tracker.Models;

namespace Expense_Tracker.Contracts
{
    public interface ICategoryRepository
    {
        Task AddCategory(Category category);
        Task<IEnumerable<Category>> GetAllCategories();
        Task<Category> GetCategory(Guid id);
        Task UpdateCategory(Guid id, Category updatedCategory);
        Task DeleteCategory(Guid id);
    }
}