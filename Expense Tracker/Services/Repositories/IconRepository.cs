using Expense_Tracker.Contracts;
using Expense_Tracker.Data;
using Expense_Tracker.Models;
using Microsoft.EntityFrameworkCore;

namespace Expense_Tracker.Services.Repositories
{
    public class IconRepository : IIconRepository
    {
        private readonly ExpenseTrackerDbContext _dbContext;

        public IconRepository(ExpenseTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<Icon>> GetAllIcons()
        {
            var icons = await _dbContext.Icons.ToListAsync();
            
            return icons;
        }
    }
}
