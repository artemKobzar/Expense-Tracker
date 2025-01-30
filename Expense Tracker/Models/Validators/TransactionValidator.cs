using FluentValidation;

namespace Expense_Tracker.Models.Validators
{
    public class TransactionValidator: AbstractValidator<Transaction>
    {
        public TransactionValidator() 
        {
            RuleFor(t => t.CategoryId).NotEmpty().WithMessage("Please select a category").NotNull();
            RuleFor(t => t.Amount).GreaterThan(0).WithMessage("{PropertyName} should be greater than 0").NotNull().WithMessage("{PropertyName} should be greater than 0");
            RuleFor(t => t.Note).MaximumLength(100).WithMessage("Shouldn't exceed 100 characters");
        }
    }
}
