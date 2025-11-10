using FluentValidation;

namespace YardManagementApplication.Validators
{

        public class VehicleModelValidator : AbstractValidator<VehicleModel>
        {
            /// <summary>
            /// Initializes validation rules for Vehicle Model fields.
            /// </summary>
            public VehicleModelValidator()
            {
                RuleFor(x => x.Brand_id)
                .NotNull().WithMessage("Brand ID is mandatory")
                .GreaterThan(0).WithMessage("Brand ID must be greater than zero");

                RuleFor(x => x.Vehicle_type_id)
                .NotNull().WithMessage("Vehicle Type ID is mandatory")
                .GreaterThan(0).WithMessage("Vehicle Type ID must be greater than zero");

                RuleFor(x => x.Variant_code)
                    .NotEmpty().WithMessage("Variant Code is mandatory")
                    .MaximumLength(50).WithMessage("Variant Code cannot exceed 50 characters")
                    .Matches(@"^[A-Za-z0-9]+$").WithMessage("Variant Code must contain only letters and number (0-9)");

            RuleFor(x => x.Variant_name)
                   .NotEmpty().WithMessage("Variant Name is mandatory")
                   .MaximumLength(100).WithMessage("Variant Name cannot exceed 100 characters")
                   .Matches(@"^[A-Za-z0-9 ]+$").WithMessage("Variant Name must contain only letters, number and space");

            RuleFor(x => x.Model_year)
                    .InclusiveBetween(1886, DateTime.Now.Year + 1)
                    .WithMessage("Invalid Model Year. Must be between 1886 and next years");

                //RuleFor(x => x.Description)
                //    .MaximumLength(250).WithMessage("Description cannot exceed 250 characters")
                //    .Matches(@"^[A-Za-z0-9 ]+$").When(x => !string.IsNullOrEmpty(x.Description))
                //    .WithMessage("Description must contain only letters,numbers and space");

                RuleFor(x => x.Wmi_code)
                    .NotEmpty().WithMessage("WMI Code is mandatory")
                    .Length(3).WithMessage("WMI Code must be 3 characters")
                    .Matches(@"^[A-Za-z0-9]+$").WithMessage("WMI Code must contain only letters and numbers (0-9)");

                RuleFor(x => x.Attr_4_8)
                    .NotEmpty().WithMessage("Vehicle Attributes  is mandatory")
                    .MaximumLength(3).WithMessage("Vehicle Attributes  cannot exceed 3 characters")
                    .Matches(@"^[A-Za-z0-9]+$").WithMessage("Vehicle Attributes  must contain only letters and numbers (0-9)");

                RuleFor(x => x.Check_digit)
                    .NotEmpty().WithMessage("Check Digit is mandatory")
                    .Length(1).WithMessage("Check Digit must be 1 character")
                    .Matches(@"^[A-Za-z0-9]+$").WithMessage("Check Digit must be alphanumeric");

                RuleFor(x => x.Model_year_code)
                    .NotEmpty().WithMessage("Model Year Code is mandatory")
                    .Length(1).WithMessage("Model Year Code must be 1 character")
                    .Matches(@"^[A-Za-z0-9]+$").WithMessage("Model Year Code must  contain only letters and numbers (0-9)");

                RuleFor(x => x.Plant_code)
                    .NotEmpty().WithMessage("Plant Code is mandatory")
                    .Length(3).WithMessage("Plant Code must be 3 character")
                    .Matches(@"^[A-Za-z0-9]+$").WithMessage("Plant Code must contain only letters and numbers (0-9)");

                RuleFor(x => x.Next_sequence_cnt)
                    .GreaterThan(0).WithMessage("Next Sequence Count must be greater than zero");

                RuleFor(x => x.Status_id)
                    .NotNull().WithMessage("Status ID is mandatory")
                    .GreaterThan(0).WithMessage("Status ID must be greater than zero");

                RuleFor(x => x.Created_by)
                    .NotEmpty().WithMessage("Created By is mandatory")
                    .MaximumLength(50).WithMessage("Created By cannot exceed 50 characters")
                    .Matches(@"^[A-Za-z0-9 ]+$").WithMessage("Created By must contain only letters name and space");
            }
        }
}
