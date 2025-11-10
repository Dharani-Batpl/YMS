using FluentValidation;



namespace YardManagementApplication.Validators

{
    public class ShiftValidator : AbstractValidator<ShiftModel>
    {





        public ShiftValidator()
        {
            RuleFor(x => x.Shift_name)
             .NotEmpty().WithMessage("Shift Name is mandatory")
             .MaximumLength(50).WithMessage("Shift Name cannot exceed 50 characters")
             .Matches(@"^[A-Za-z0-9 ]+$").WithMessage("Shift Name must contain only letters,numbers and space");

            //RuleFor(x => x.Shift_description)
            //    .MaximumLength(250).WithMessage("Shift Description cannot exceed 250 characters")
            //    .Matches(@"^[A-Za-z0-9 ]+$").WithMessage("Shift Description must contain only letters,numbers and space");

            RuleFor(x => x.Status_id)
                .GreaterThan(0).WithMessage("Valid Status Id is mandatory");


            RuleFor(x => x.Created_by)
                .NotEmpty().WithMessage("Created By is mandatory")
                .MaximumLength(50).WithMessage("Created By cannot exceed 50 characters")
                .Matches(@"^[A-Za-z0-9 ]+$").WithMessage("Created By must contain only letters,numbers and space");

        }
    }

}
