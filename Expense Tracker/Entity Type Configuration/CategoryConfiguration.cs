using Expense_Tracker.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Expense_Tracker.Entity_Type_Configuration
{
    public class CategoryConfiguration: IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.Property(c => c.Title).HasMaxLength(50).IsRequired().HasColumnType("NVARCHAR(50)");
            builder.Property(c => c.Type).HasMaxLength(50).IsRequired().HasColumnType("NVARCHAR(50)");
            builder.HasOne(c => c.Icon).WithMany().HasForeignKey(c => c.IconId).OnDelete(DeleteBehavior.SetNull);

            builder.ToTable(nameof(Category));
        }
    }
}
