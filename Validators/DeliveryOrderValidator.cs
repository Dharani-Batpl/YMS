// AppUserValidator.cs
using FluentValidation;
using YardManagementApplication.Models;

namespace YardManagementApplication.Validators
{
    public class DeliveryOrderValidator : AbstractValidator<DeliveryOrderModel>
    {
        public DeliveryOrderValidator()
        {
            RuleFor(x => x.Do_number)
                .NotEmpty().WithMessage("DO Number is required")
                .MaximumLength(50).WithMessage("DO Number cannot exceed 50 characters")
                .Matches(@"^[A-Za-z0-9_-]+$").WithMessage("DO Number may contain letters, numbers, hyphens (-), and underscores (_).");

            RuleFor(x => x.Customer_id)
                .GreaterThan(0).WithMessage("Customer Id must be valid");

            RuleFor(x => x.Delivery_location_id)
                .GreaterThan(0).WithMessage("Delivery Location Id must be valid");

            RuleFor(x => x.Priority)
                .NotEmpty().WithMessage("Priority is required")
                .MaximumLength(50).WithMessage("Priority cannot exceed 50 characters");

            RuleFor(x => x.Planned_dispatch_at)
                .NotEmpty().WithMessage("Planned Dispatch Time is required");

            RuleFor(x => x.Created_by)
                .NotEmpty().WithMessage("Created By is required")
                .MaximumLength(50).WithMessage("Created By cannot exceed 50 characters")
                .Matches(@"^[A-Za-z0-9 ]+$").WithMessage("Created By must contain only letters and numbers (0-9)");

            RuleForEach(x => x.Details)
                .SetValidator(new DeliveryOrderDetailValidator());
        }


        public class DeliveryOrderDetailValidator : AbstractValidator<DeliveryOrderDetailModel>
        {
            public DeliveryOrderDetailValidator()
            {
                RuleFor(x => x.Color_id)
                    .GreaterThan(0).WithMessage("Color Id must be valid");

                RuleFor(x => x.Brand_id)
                    .GreaterThan(0).WithMessage("Brand Id must be valid");

                RuleFor(x => x.Vehicle_type_id)
                   .GreaterThan(0).WithMessage("Vehicle Type Id must be valid");

                RuleFor(x => x.Variant_id)
                   .GreaterThan(0).WithMessage("Variant Id must be valid");

                RuleFor(x => x.Quantity_ordered)
                    .GreaterThan(0).WithMessage("Quantity Ordered must be greater than 0");

                RuleFor(x => x.Created_by)
                    .NotEmpty().WithMessage("Created By is required")
                    .MaximumLength(50).WithMessage("Created By cannot exceed 50 characters")
                    .Matches(@"^[A-Za-z0-9 ]+$").WithMessage("Created By must contain only letters and numbers (0-9)");
            }
        }
    }
}
