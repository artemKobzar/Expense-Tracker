using Expense_Tracker.Models;

namespace Expense_Tracker.Contracts
{
    public interface IIconRepository
    {
        Task<IEnumerable<Icon>> GetAllIcons();
    }
}