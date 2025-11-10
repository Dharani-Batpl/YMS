using FluentValidation;



namespace YardManagementApplication.Validators

{
    public class DepartmentValidator : AbstractValidator<DepartmentModel>
    {
        /// <summary>
        /// Defines validation rules for Department fields.
        /// </summary>
        public DepartmentValidator()
        {
            RuleFor(x => x.Department_code)
                .NotEmpty().WithMessage("Department Code is mandatory")
                .MaximumLength(50).WithMessage("Department Code cannot exceed 50 characters")
                .Matches(@"^[A-Za-z0-9]+$").WithMessage("Department Code must contain only letters and numbers (0-9)");

            RuleFor(x => x.Department_name)
                .NotEmpty().WithMessage("Department Name is mandatory")
                .MaximumLength(100).WithMessage("Department Name cannot exceed 100 characters")
                .Matches(@"^[A-Za-z0-9]+$").WithMessage("Department Name must contain only letters and numbers (0-9)");

            //RuleFor(x => x.Description)
            //    .MaximumLength(250).When(x => x.Description != null)
            //    .WithMessage("Description cannot exceed 250 characters")
            //    .Matches(@"^[A-Za-z0-9]+$").When(x => !string.IsNullOrEmpty(x.Description))
            //    .WithMessage("Description must contain only letters and numbers (0-9)");

            RuleFor(x => x.Status_id)
                .GreaterThan(0).WithMessage("Valid Status Id is mandatory");

            RuleFor(x => x.Created_by)
                .NotEmpty().WithMessage("Created By is mandatory")
                .MaximumLength(50).WithMessage("Created By cannot exceed 50 characters")
                .Matches(@"^[A-Za-z0-9]+$").WithMessage("Created By must contain only letters and numbers (0-9)");


        }
    }

}
