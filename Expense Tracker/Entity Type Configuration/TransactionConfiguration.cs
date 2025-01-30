using Expense_Tracker.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Expense_Tracker.Entity_Type_Configuration
{
    public class TransactionConfiguration: IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasKey(t => t.Id);
            builder.HasIndex(t => t.Id).IsUnique();
            builder.Property(t => t.Amount).HasColumnType("DECIMAL(18, 2)").IsRequired();
            builder.Property(t => t.Note).HasMaxLength(100).HasColumnType("NVARCHAR(100)");
            builder.HasOne(c => c.Category).WithMany(t => t.Transactions).HasForeignKey(c => c.CategoryId).IsRequired().OnDelete(DeleteBehavior.Cascade);

            builder.ToTable(nameof(Transaction));
        }
    }
}
