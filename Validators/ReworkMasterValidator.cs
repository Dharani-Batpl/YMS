using FluentValidation;
using System.Text.RegularExpressions;



namespace YardManagementApplication.Validators

{
    public class ReworkMasterValidator : AbstractValidator<ReWorkModel>
    {
        /// <summary>
        /// Builds the validation rules.
        /// </summary>
        public ReworkMasterValidator()
        {
            // ===== Mandatory (NOT NULL in table) =====
            RuleFor(x => x.Vin)
                .NotEmpty().WithMessage("VIN is mandatory")
                .Length(17).WithMessage("VIN must be exactly 17 characters")
                .Matches("^(?=.*[A-Z])(?=.*[0-9])[A-Z0-9]{17}$")
                .WithMessage("VIN must be 17 characters long, contain only capital letters (A–Z) and numbers (0–9), and include at least one letter and one number.");

            //RuleFor(x => x.Brand_id)
            //    .NotNull().WithMessage("Brand ID is mandatory");

            //RuleFor(x => x.Vehicle_type_id)
            //    .NotNull().WithMessage("Vehicle Type ID is mandatory");

            //RuleFor(x => x.Variant_id)
            //    .NotNull().WithMessage("Variant ID is mandatory");


            RuleFor(x => x.Created_by)
                .NotEmpty().WithMessage("Created By is mandatory")
                .MaximumLength(50).WithMessage("Created By cannot exceed 50 characters")
                .Matches(@"^[A-Za-z0-9 ]+$").WithMessage("Created By must contain only letters,space and numbers (0-9)");


        }

        /// <summary>
        /// Returns true if the value contains only uppercase alphanumeric characters (A–Z, 0–9).
        /// </summary>
        private static bool AlphaNumUpperOnly(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            s = s.ToUpperInvariant();
            return !Regex.IsMatch(s, @"[^A-Z0-9]");
        }
    }

}
