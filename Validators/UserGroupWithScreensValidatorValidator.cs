// UserGroupWithScreensValidator.cs
using FluentValidation;
using YardManagementApplication.Models;

namespace YardManagementApplication.Validators
{
    public class UserGroupWithScreensValidator : AbstractValidator<UserGroupModel>
    {
        public UserGroupWithScreensValidator()
        {
            RuleFor(x => x.User_group_name)
                .NotEmpty().WithMessage("User Group Name is required")
                .MaximumLength(100).WithMessage("User Group Name cannot exceed 100 characters")
                .Matches(@"^[A-Za-z0-9 ]+$").WithMessage("User Group Name must contain only letters, numbers and space");

            //RuleFor(x => x.Description)
            //     .NotEmpty().WithMessage("Description is required")
            //     .MaximumLength(100).WithMessage("Description cannot exceed 100 characters")
            //     .Matches(@"^[A-Za-z0-9 ]+$").WithMessage("Description must contain only letters, numbers and space");

            RuleFor(x => x.Created_by)
                .NotEmpty().WithMessage("Created By is mandatory")
                .MaximumLength(50)
                .Matches(@"^[A-Za-z0-9 ]+$").WithMessage("Created By must contain only letters, numbers and space");
        }
    }
}
