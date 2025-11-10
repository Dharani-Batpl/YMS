using FluentValidation;

namespace YardManagementApplication.Validators
{

    public class EmployeeMasterValidator : AbstractValidator<EmployeeModel>
    {
        public EmployeeMasterValidator()
        {
            RuleFor(x => x.Employee_code)
                .NotEmpty().WithMessage("Employee Code is mandatory")
                .MaximumLength(50).WithMessage("Employee Code cannot exceed 50 characters")
                .Matches(@"^[A-Za-z0-9 ]+$").WithMessage("Employee Code can contain only letters (A–Z, a–z) and digits (0–9).");

            RuleFor(x => x.First_name)
                .NotEmpty().WithMessage("First Name is mandatory")
                .MaximumLength(100).WithMessage("First Name cannot exceed 100 characters")
                .Matches(@"^[A-Za-z \s]+$").WithMessage("First Name can contain only letters (A–Z, a–z).");

            RuleFor(x => x.Last_name)
                .NotEmpty().WithMessage("Last Name is mandatory")
                .MaximumLength(100).WithMessage("Last Name cannot exceed 100 characters")
                .Matches(@"^[A-Za-z \s]+$").WithMessage("Last Name can contain only letters (A–Z, a–z).");

            RuleFor(x => x.Department_name)
                .NotEmpty().WithMessage("Department Name is mandatory")
                .MaximumLength(100).WithMessage("Department Name cannot exceed 100 characters");

            RuleFor(x => x.Contact_number)
                .NotEmpty().WithMessage("Contact Number is mandatory")
                .MaximumLength(20).WithMessage("Contact Number cannot exceed 20 characters")
                .Matches(@"^[0-9]+$").WithMessage("Contact Number must contain only digits (0–9).");

            RuleFor(x => x.Emergency_contact_number)
                .NotEmpty().WithMessage("Emergency_contact_number is mandatory")
                .MaximumLength(20).WithMessage("Contact Number cannot exceed 20 characters")
                .Matches(@"^[0-9]+$").WithMessage("Contact Number must contain only digits (0–9).");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is mandatory")
                .MaximumLength(150).WithMessage("Email cannot exceed 150 characters")
                .EmailAddress().WithMessage("Email must be a valid email address");

            RuleFor(x => x.Employee_type)
                .NotEmpty().WithMessage("Employee Type is mandatory")
                .MaximumLength(30).WithMessage("Employee Type cannot exceed 30 characters")
                .Matches(@"^[A-Za-z0-9 ]+$").WithMessage("Employee Type can contain only letters (A–Z, a–z) and digits (0–9).");
            RuleFor(x => x.Certificate_type_name)
            .NotEmpty().WithMessage("Certificate Type is mandatory")
            .MaximumLength(100).WithMessage("Certificate Type cannot exceed 100 characters")
            .Matches(@"^[A-Za-z0-9 ]+$").WithMessage("Certificate Type can contain only letters (A–Z, a–z), digits (0–9) and spaces.");


            //RuleFor(x => x.User_name)
            //    .NotEmpty().WithMessage("User Name is mandatory")
            //    .MaximumLength(100).WithMessage("User Name cannot exceed 100 characters")
            //    .Matches(@"^[A-Za-z0-9 ]+$").WithMessage("User Name can contain only letters (A–Z, a–z) and digits (0–9).");

            //RuleFor(x => x.User_group_name)
            //    .NotEmpty().WithMessage("User Group Name is mandatory")
            //    .MaximumLength(100).WithMessage("User Group Name cannot exceed 100 characters");

            RuleFor(x => x.Skill_type)
            .NotEmpty().WithMessage("Skill Type is mandatory")
            .MaximumLength(100).WithMessage("Skill Type cannot exceed 100 characters")
            .Matches(@"^[A-Za-z0-9\s]+$").WithMessage("Skill Type can contain only letters (A–Z, a–z), digits (0–9), and spaces.");

            RuleFor(x => x.Skill_level_name)
                .NotEmpty().WithMessage("Skill Level Name is mandatory")
                .MaximumLength(100).WithMessage("Skill Level Name cannot exceed 100 characters")
                .Matches(@"^[A-Za-z0-9 \s]+$").WithMessage("Skill Level Name can contain only letters (A–Z, a–z), digits (0–9), and spaces.");

            //RuleFor(x => x.Certification_date)
            //    .NotEmpty().WithMessage("Certification Date is mandatory");
            //RuleFor(x => x.Address)
            //    .NotEmpty().WithMessage("Address is mandatory");

            //RuleFor(x => x.Expiry_date)
            //    .NotEmpty().WithMessage("Expiry Date is mandatory");

            RuleFor(x => x.Status_name)
                .NotEmpty().WithMessage("Status Name is mandatory")
                .MaximumLength(100).WithMessage("Status Name cannot exceed 100 characters")
                .Matches(@"^[A-Za-z0-9 \s]+$").WithMessage("Status Name can contain only letters (A–Z, a–z), digits (0–9), and spaces.");

        }
    }
}

