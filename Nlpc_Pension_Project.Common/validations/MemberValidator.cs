using FluentValidation;
using Nlpc_Pension_Project.Application.Dtos;

namespace Nlpc_Pension_Project.Common.validations;

// MemberValidator.cs
public class MemberValidator : AbstractValidator<CreateMemberDto>
{
    public MemberValidator()
    {
        RuleFor(m => m.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(m => m.LastName).NotEmpty().MaximumLength(50);
        RuleFor(m => m.Email).NotEmpty().EmailAddress().MaximumLength(100);
        RuleFor(m => m.DateOfBirth).Must(BeValidAge).WithMessage("Age must be between 18 and 70");
        RuleFor(m => m.PhoneNumber).Matches(@"^\+?[1-9]\d{1,14}$").When(x => !string.IsNullOrEmpty(x.PhoneNumber));
    }

    private bool BeValidAge(DateTime dateOfBirth)
    {
        var age = DateTime.Today.Year - dateOfBirth.Year;
        if (dateOfBirth > DateTime.Today.AddYears(-age)) age--;
        return age >= 18 && age <= 70;
    }
}
