using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Expense_Tracker.Data
{
    public class ExpenseTrackerDbContextFactory:IDesignTimeDbContextFactory<ExpenseTrackerDbContext>
    {
        public ExpenseTrackerDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var builder = new DbContextOptionsBuilder<ExpenseTrackerDbContext>();
            var connectionString = configuration.GetConnectionString("SQLDbConnection");

            builder.UseSqlServer(connectionString);

            return new ExpenseTrackerDbContext(builder.Options);
        }
    }
}
