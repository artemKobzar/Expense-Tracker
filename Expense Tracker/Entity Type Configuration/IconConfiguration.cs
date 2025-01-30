using Expense_Tracker.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Expense_Tracker.Entity_Type_Configuration
{
    public class IconConfiguration: IEntityTypeConfiguration<Icon>
    {
        public void Configure(EntityTypeBuilder<Icon> builder)
        {
            builder.HasKey(i => i.Id);
            builder.Property(i => i.Name).HasMaxLength(50).IsRequired().HasColumnType("NVARCHAR(50)");
            builder.Property(i => i.Logo).HasMaxLength(5).HasColumnType("NVARCHAR(10)");

            builder.ToTable(nameof(Icon));
        }
    }
}
