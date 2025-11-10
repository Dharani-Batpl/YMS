using FluentValidation;

namespace YardManagementApplication.Validators
{
    public class VehicleTypeValidatorcs
    {
        public class VehicleTypeValidator : AbstractValidator<VehicleTypeModel>
        {
            /// <summary>
            /// Initializes validation rules for Vehicle Type master fields.
            /// </summary>
            public VehicleTypeValidator()
            {

                  RuleFor(x => x.Brand_id)
      .GreaterThan(0).WithMessage("Valid Brand Id is mandatory");

  RuleFor(x => x.Vehicle_type_code)
      .NotEmpty().WithMessage("Vehicle Type Code is mandatory")
      .MaximumLength(50).WithMessage("Vehicle Type Code cannot exceed 50 characters")
      .Matches(@"^[A-Za-z0-9]+$").WithMessage("Vehicle Type Code can contain only letters and numbers");

  RuleFor(x => x.Vehicle_type_name)
      .NotEmpty().WithMessage("Vehicle Type Name is mandatory")
      .MaximumLength(100).WithMessage("Vehicle Type Name cannot exceed 100 characters")
      .Matches(@"^[A-Za-z0-9 ]+$").WithMessage("Vehicle Type Name can contain only letters, numbers and space");

  RuleFor(x => x.Category_type_id)
      .GreaterThan(0).WithMessage("Valid Category Type Id is mandatory");

  RuleFor(x => x.Fuel_type_id)
      .GreaterThan(0).WithMessage("Valid Fuel Type Id is mandatory");

  RuleFor(x => x.Status_id)
      .GreaterThan(0).WithMessage("Valid Status Id is mandatory");

  //RuleFor(x => x.Description)
  //    .MaximumLength(250).WithMessage("Vehicle Type Description cannot exceed 250 characters")
  //    .Matches(@"^[A-Za-z0-9 ]*$").WithMessage("Description can contain only letters, numbers and space");
            }
        }

    }
}
