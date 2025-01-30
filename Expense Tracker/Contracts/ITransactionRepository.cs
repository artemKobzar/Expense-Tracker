using Expense_Tracker.Models;

namespace Expense_Tracker.Contracts
{
    public interface ITransactionRepository
    {
        Task AddTransaction(Transaction transaction);
        Task DeleteTransaction(Guid id);
        Task<IEnumerable<Transaction>> GetAllTransactions();
        Task<Transaction> GetTransaction(Guid id);
        Task UpdateTransaction(Guid id, Transaction updatedTransaction);
    }
}