using FluentValidation;
using YardManagementApplication.Models;

namespace YardManagementApplication.Validators
{
    public class BreakTimeValidator : AbstractValidator<BreakTimeModel>
    {
        public BreakTimeValidator()
        {
            RuleFor(x => x.Break_name)
                .NotEmpty().WithMessage("Break Name is mandatory")
                .MaximumLength(25).WithMessage("Break Name cannot exceed 25 characters")
                .Matches(@"^[A-Za-z0-9 ]+$").WithMessage("Break Name must contain only letters, numbers, and spaces");

            //RuleFor(x => x.Break_description)
            //    .MaximumLength(25).WithMessage("Break Description cannot exceed 25 characters")
            //    .Matches(@"^[A-Za-z0-9 ]*$").WithMessage("Break Description must contain only letters, numbers, and spaces");

            RuleFor(x => x.Status_id)

              .NotNull().WithMessage("Status ID is mandatory")

              .GreaterThan(0).WithMessage("Status ID must be greater than zero");

            RuleFor(x => x.Created_by)
                .NotEmpty().WithMessage("Created By is mandatory")
                .MaximumLength(50).WithMessage("Created By cannot exceed 50 characters")
                .Matches(@"^[A-Za-z0-9 ]+$").WithMessage("Created By must contain only letters, numbers, and spaces");
        }
    }
}
