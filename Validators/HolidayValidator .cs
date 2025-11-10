using FluentValidation;



namespace YardManagementApplication.Validators

{
    public class HolidayValidator : AbstractValidator<HolidayModel>
    {
        public HolidayValidator()
        {
            RuleFor(x => x.Holiday_name)
                .NotEmpty().WithMessage("Holiday Name is mandatory")
                .MaximumLength(150).WithMessage("Holiday Name cannot exceed 150 characters")
                .Matches(@"^[A-Za-z0-9 ]+$").WithMessage("Holiday Name must contain only letters, numbers and space");

            RuleFor(x => x.Holiday_date)
                .NotEmpty().WithMessage("Holiday Date is mandatory");

            RuleFor(x => x.Holiday_type_id)
               .GreaterThan(0).WithMessage("Valid Holiday_type_Id is mandatory");    

            RuleFor(x => x.Status_id)
                 .GreaterThan(0).WithMessage("Valid Status Id is mandatory");

            RuleFor(x => x.Created_by)
                .NotEmpty().WithMessage("Created By is mandatory")
                .MaximumLength(50).WithMessage("Created By cannot exceed 50 characters")
                .Matches(@"^[A-Za-z0-9 ]+$").WithMessage("Created By must contain only letters, numbers and space");

            //RuleFor(x => x.Description)
            //    .NotEmpty().WithMessage("Description is Mandatory")
            //    .MaximumLength(250).WithMessage("Enterprise Description cannot exceed 250 characters")
            //    .Matches(@"^[A-Za-z0-9 ]+$").WithMessage("Description must contain only letters, numbers and space");
        }
    }

}
