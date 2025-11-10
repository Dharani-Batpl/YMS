using FluentValidation;
using System.Text.RegularExpressions;



namespace YardManagementApplication.Validators

{
    public class EolMasterValidator : AbstractValidator<EolProductionModel>
    {
        /// <summary>
        /// Builds the validation rules.
        /// </summary>
        public EolMasterValidator()
        {
            // ===== Mandatory (NOT NULL in table) =====
            RuleFor(x => x.Vin)
               .NotEmpty().WithMessage("VIN is mandatory")
               .Length(17).WithMessage("VIN must be exactly 17 characters")
               .Matches("^(?=.*[A-Z])(?=.*[0-9])[A-Z0-9]{17}$")
               .WithMessage("VIN must be 17 characters long, contain only capital letters (A–Z) and numbers (0–9), and include at least one letter and one number.");


            RuleFor(x => x.Production_order_id)
                .NotEmpty().WithMessage("Production Order ID is mandatory")
                .MaximumLength(50).WithMessage("Production Order ID cannot exceed 50 characters")
                .Matches(@"^[A-Za-z0-9]+$").WithMessage("Production Order ID must contain only letters and numbers (0-9)");

            RuleFor(x => x.Eol_quality_inspector_id)
                .NotEmpty().WithMessage("EOL Quality Inspector ID is mandatory")
                .MaximumLength(50).WithMessage("EOL Quality Inspector ID cannot exceed 50 characters")
                .Matches(@"^[A-Za-z0-9]+$").WithMessage("EOL Quality Inspector ID must contain only letters and numbers (0-9)");

            //RuleFor(x => x.Brand_id)
            //    .NotNull().WithMessage("Brand ID is mandatory");

            //RuleFor(x => x.Vehicle_type_id)
            //    .NotNull().WithMessage("Vehicle Type ID is mandatory");

            //RuleFor(x => x.Variant_id)
            //    .NotNull().WithMessage("Variant ID is mandatory");

            //RuleFor(x => x.Line_id)
            //    .NotEmpty().WithMessage("Line ID is mandatory")
            //    .MaximumLength(50).WithMessage("Line ID cannot exceed 50 characters")
            //    .Matches(@"^[A-Za-z0-9]+$").WithMessage("Line ID must contain only letters and numbers (0-9)");

            RuleFor(x => x.Shop_id)
                .NotEmpty().WithMessage("Shop ID is mandatory")
                .MaximumLength(50).WithMessage("Shop ID cannot exceed 50 characters")
                .Matches(@"^[A-Za-z0-9]+$").WithMessage("Shop ID must contain only letters and numbers (0-9)");

            RuleFor(x => x.Created_by)
                .NotEmpty().WithMessage("Created By is mandatory")
                .MaximumLength(50).WithMessage("Created By cannot exceed 50 characters")
                .Matches(@"^[A-Za-z0-9]+$").WithMessage("Created By must contain only letters and numbers (0-9)");


            RuleFor(x => x.Certificate_id)
                .MaximumLength(50).When(x => x.Certificate_id != null)
                .WithMessage("Certificate ID cannot exceed 50 characters")
                .Matches(@"^[A-Za-z0-9]+$").When(x => !string.IsNullOrEmpty(x.Certificate_id))
                .WithMessage("Certificate ID must contain only letters and numbers (0-9)");

            //RuleFor(x => x.Quality_status)
            //    .InclusiveBetween(0, 2).When(x => x.Quality_status.HasValue)
            //    .WithMessage("Invalid Quality Status");

            //RuleFor(x => x.Transit_status)
            //    .InclusiveBetween(0, 2).When(x => x.Transit_status.HasValue)
            //    .WithMessage("Invalid Transit Status");
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
