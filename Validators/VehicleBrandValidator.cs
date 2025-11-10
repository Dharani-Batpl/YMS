using FluentValidation;

namespace YardManagementApplication.Validators
{
    public class VehicleBrandValidator : AbstractValidator<VehicleBrandModel>
    {
        public VehicleBrandValidator()
        {
            RuleFor(x => x.Brand_code)
                .NotEmpty().WithMessage("Brand Code is mandatory")
                .MaximumLength(50).WithMessage("Brand Code cannot exceed 50 characters")
                .Matches(@"^[A-Za-z0-9]+$").WithMessage("Brand Code must contain only letters and numbers (0-9)");

            RuleFor(x => x.Brand_name)
                .NotEmpty().WithMessage("Brand Name is mandatory")
                .MaximumLength(100).WithMessage("Brand Name cannot exceed 100 characters")
                .Matches(@"^[A-Za-z0-9 ]+$").WithMessage("Brand Name must contain only letters , numbers (0-9) and space");

            RuleFor(x => x.Country_name)
                .NotEmpty().WithMessage("Country Name is mandatory")
                .MaximumLength(100).WithMessage("Country Name cannot exceed 100 characters")
                .Matches(@"^[A-Za-z0-9]+$").WithMessage("Country Name must contain only letters and numbers (0-9)");

            RuleFor(x => x.Status_name)
                .NotEmpty().WithMessage("Status Name is mandatory")
                .MaximumLength(100).WithMessage("Status Name cannot exceed 100 characters")
                .Matches(@"^[A-Za-z0-9]+$").WithMessage("Status Name must contain only letters and numbers (0-9)");

            //RuleFor(x => x.Description)
            //    .MaximumLength(250).WithMessage("Brand Description cannot exceed 250 characters")
            //    .Matches(@"^[A-Za-z0-9 ]+$").When(x => !string.IsNullOrEmpty(x.Description))
            //    .WithMessage("Brand Description must contain only letters ,numbers (0-9) and space ");

            RuleFor(x => x.Created_by)
                .NotEmpty().WithMessage("Created By is mandatory")
                .MaximumLength(50).WithMessage("Created By cannot exceed 50 characters")
                .Matches(@"^[A-Za-z0-9]+$").WithMessage("Created By must contain only letters and numbers (0-9)");
        }
    }
}
