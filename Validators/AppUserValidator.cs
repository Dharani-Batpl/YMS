// AppUserValidator.cs
using FluentValidation;
using YardManagementApplication.Models;

namespace YardManagementApplication.Validators
{
    public class AppUserValidator : AbstractValidator<AppUserModel>
    {
        public AppUserValidator()
        {
            //RuleFor(x => x.Email)
            //    .NotEmpty().WithMessage("Email is mandatory")
            //    .EmailAddress().WithMessage("Email must be a valid email address");

            //RuleFor(x => x.Password)
            // .NotEmpty().WithMessage("Password is required")
            // .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")
            // .WithMessage("Password must be at least 8 characters long, and include at least one uppercase letter, one lowercase letter, one number, and one special character.");

            //RuleFor(x => x.Employee_id)
            //     .NotNull().WithMessage("Employee ID cannot be null")
            //     .NotEmpty().WithMessage("Employee ID is mandatory")
            //     .Matches(@"^[A-Za-z0-9 ]+$").WithMessage("Employee ID must contain only letters, numbers and space");

            //RuleFor(x => x.First_name)
            //     .MaximumLength(100).When(x => x.First_name != null)
            //     .WithMessage("First Name cannot exceed 100 characters")
            //     .Matches(@"^[A-Za-z0-9 ]+$").When(x => x.First_name != null)
            //     .WithMessage("First Name must contain only letters, numbers and space");

            //RuleFor(x => x.Last_name)
            //     .MaximumLength(100).When(x => x.Last_name != null)
            //     .WithMessage("Last Name cannot exceed 100 characters")
            //     .Matches(@"^[A-Za-z0-9 ]+$").When(x => x.Last_name != null)
            //     .WithMessage("Last Name must contain only letters, numbers and space");

            ////RuleFor(x => x.Contact_number)
            ////    .MaximumLength(20).When(x => x.Contact_number != null)
            ////    .WithMessage("Contact Number cannot exceed 20 characters")
            ////    .Matches(@"^[0-9]+$").When(x => x.Contact_number != null)
            ////    .WithMessage("Contact Number must contain only digits (0–9).");

            //RuleFor(x => x.Department_name)
            //     .NotNull().WithMessage("Department Id is required");


            //RuleFor(x => x.User_group_name)
            //     .NotNull().WithMessage("User Group Id is required");


            //RuleFor(x => x.Status_name)
            //     .NotNull().WithMessage("Status Id is required");


            //RuleFor(x => x.Created_by)
            //    .NotEmpty().WithMessage("Created By is mandatory")
            //    .MaximumLength(50).WithMessage("Created By cannot exceed 50 characters")
            //    .Matches(@"^[A-Za-z0-9 ]+$").WithMessage("Created By must contain only letters, numbers and space");
        }
    }
}
