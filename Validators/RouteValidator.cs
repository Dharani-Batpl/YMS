using FluentValidation;



namespace YardManagementApplication.Validators

{
    public class RouteValidator : AbstractValidator<RouteModel>
    {
        /// <summary>
        /// Initializes validation rules for the Route master fields.
        /// </summary>
        public RouteValidator()
        {
            // Allow common punctuation often present in area/status names.
            const string namePattern = @"^[A-Za-z0-9 ]+$";

            RuleFor(x => x.Status_id)

            .NotNull().WithMessage("Status ID is mandatory")

            .GreaterThan(0).WithMessage("Status ID must be greater than zero");


            RuleFor(x => x.Source_process_area_id)
                .NotNull().WithMessage("Status ID is mandatory")

            .GreaterThan(0).WithMessage("Source Process Area id is mandatory");

            RuleFor(x => x.Destination_process_area_id)
                .NotNull().WithMessage("Status ID is mandatory")

            .GreaterThan(0).WithMessage("Destination Process Area id is mandatory");


            RuleFor(x => x.Sla_minutes_cnt)
                .GreaterThanOrEqualTo(0).WithMessage("SLA Minutes Count must be 0 or more");

            RuleFor(x => x.Created_by)
                .NotEmpty().WithMessage("Created By is mandatory")
                .MaximumLength(50).WithMessage("Created By cannot exceed 50 characters")
                .Matches(@"^[A-Za-z0-9 ]+$").WithMessage("Created By can contain only letters ,numbers and space");
        }
    }
}
