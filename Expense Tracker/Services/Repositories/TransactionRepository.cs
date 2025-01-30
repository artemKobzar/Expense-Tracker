using Expense_Tracker.Contracts;
using Expense_Tracker.Data;
using Expense_Tracker.Models;
using Microsoft.EntityFrameworkCore;

namespace Expense_Tracker.Services.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ExpenseTrackerDbContext _dbContext;

        public TransactionRepository(ExpenseTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<Transaction>> GetAllTransactions()
        {
            var transactions = await _dbContext.Transactions.Include(t => t.Category).ThenInclude(c => c.Icon).ToListAsync();
            return transactions;
        }
        public async Task<Transaction> GetTransaction(Guid id)
        {
            var transaction = await _dbContext.Transactions.Include(c => c.Category).FirstOrDefaultAsync(c => c.Id == id);
            if (transaction == null)
            {
                throw new KeyNotFoundException("Transaction not found");
            }

            return transaction;
        }
        public async Task AddTransaction(Transaction transaction)
        {
            var category = await _dbContext.Categories.FindAsync(transaction.CategoryId);
            if (category != null)
            {
                transaction.Category = category;
            }
            else
            {
                // Handle case when Icon is not found
                throw new InvalidOperationException("Category not found.");
            }
            _dbContext.Attach(transaction.Category);
            await _dbContext.Transactions.AddAsync(transaction);
            await _dbContext.SaveChangesAsync();
        }
        public async Task UpdateTransaction(Guid id, Transaction updatedTransaction)
        {
            if (updatedTransaction == null)
            {
                throw new ArgumentNullException(nameof(updatedTransaction));
            }
            var transaction = await _dbContext.Transactions.FindAsync(id);

            if (transaction == null)
            {
                throw new KeyNotFoundException("Transaction not found");
            }
            transaction.Amount = updatedTransaction.Amount;
            transaction.Note = updatedTransaction.Note;
            transaction.Date = updatedTransaction.Date;
            transaction.CategoryId = updatedTransaction.CategoryId;

            var category = await _dbContext.Categories.FindAsync(updatedTransaction.CategoryId);
            if (category != null)
            {
                transaction.Category = category;
            }
            else
            {
                // Handle case when Icon is not found
                throw new InvalidOperationException("Category not found.");
            }

            await _dbContext.SaveChangesAsync();
        }
        public async Task DeleteTransaction(Guid id)
        {
            var transaction = await _dbContext.Transactions.FindAsync(id);
            if (transaction == null)
            {
                throw new KeyNotFoundException("Transaction not found");
            }
            _dbContext.Transactions.Remove(transaction);
            await _dbContext.SaveChangesAsync();
        }
    }
}
