using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expense_Tracker.Models.Validators
{
    public class CategoryValidator: AbstractValidator<Category>
    {
        public CategoryValidator() 
        {
            RuleFor(c => c.Title).NotEmpty().WithMessage("{PropertyName} shouldn't be empty!").Length(1, 50)
                .WithMessage("Title must not exceed 50 characters").NotNull().WithMessage("Title cannot be null.");
        }
    }
}
