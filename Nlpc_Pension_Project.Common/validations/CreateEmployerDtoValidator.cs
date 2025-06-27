namespace Nlpc_Pension_Project.Common.validations;

using FluentValidation;
using Nlpc_Pension_Project.Application.Dtos;

public class CreateEmployerDtoValidator : AbstractValidator<CreateEmployerDto>
{
    public CreateEmployerDtoValidator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Company name is required.")
            .MaximumLength(100).WithMessage("Company name must not exceed 100 characters.");

        RuleFor(x => x.RegistrationNumber)
            .NotEmpty().WithMessage("Registration number is required.")
            .Matches(@"^RC\d{6}$") 
            .WithMessage("Registration number must be in the format 'RC123456'.");
    }
}

