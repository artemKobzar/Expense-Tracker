using Expense_Tracker.Contracts;
using Expense_Tracker.Models;
using Expense_Tracker.Models.Validators;
using Expense_Tracker.Services.PdfGenerating;
using Expense_Tracker.Services.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Expense_Tracker.Data
{
    public static class ServicesRegistration
    {
        public static IServiceCollection CofigureServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDbContext<ExpenseTrackerDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("SQLDbConnection"));
                options.EnableSensitiveDataLogging(true);
            }, ServiceLifetime.Scoped);

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IIconRepository, IconRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IDashboardRepository, DashboardRepository>();
            services.AddScoped<IValidator<Category>, CategoryValidator>();
            services.AddScoped<IValidator<Transaction>, TransactionValidator>();
            services.AddScoped<ITransactionPdfGenerate, TransactionPdfGenerate>();

            return services;
        }
    }
}
