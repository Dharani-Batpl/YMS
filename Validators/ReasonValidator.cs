using FluentValidation;



namespace YardManagementApplication.Validators

{
    public class ReasonValidator : AbstractValidator<ReasonModel>
    {
        /// <summary>
        /// Initializes validation rules for the Reason master fields.
        /// </summary>
        public ReasonValidator()
        {
                       RuleFor(x => x.Reason_code)
                .NotEmpty().WithMessage("Reason Code is mandatory")
                .MaximumLength(50).WithMessage("Reason Code cannot exceed 50 characters")
                .Matches(@"^[A-Za-z0-9]+$").WithMessage("Reason Code can contain only letters and  numbers");

            RuleFor(x => x.Reason_name)
                .NotEmpty().WithMessage("Reason Name is mandatory")
                .MaximumLength(100).WithMessage("Reason Name cannot exceed 100 characters")
                .Matches(@"^[A-Za-z0-9\s ]+$").WithMessage("Reason Name can contain only letters, numbers and space");

            RuleFor(x => x.Status_id)
               .GreaterThan(0).WithMessage("Valid Status Id is mandatory");

            RuleFor(x => x.Created_by)
                .NotEmpty().WithMessage("Created By is mandatory")
                .MaximumLength(50).WithMessage("Created By cannot exceed 50 characters")
                .Matches(@"^[A-Za-z0-9 ]+$").WithMessage("Created By can contain only letters, numbers and space");
        }
    }

}
