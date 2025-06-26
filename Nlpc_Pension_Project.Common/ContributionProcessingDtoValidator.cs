

using FluentValidation;
using Nlpc_Pension_Project.Application.Dtos;
using Nlpc_Pension_Project.Domain.Enums;

namespace Nlpc_Pension_Project.Common;

public class ContributionProcessingDtoValidator : AbstractValidator<ContributionProcessingDto>
{
    public ContributionProcessingDtoValidator()
    {
        RuleFor(x => x.Type)
            .Must(type => type == ContributionType.Voluntary || type == ContributionType.Monthly)
            .WithMessage("ContributionType must be either 'Voluntary' or 'Monthly'.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than 0.");

        RuleFor(x => x.ContributionDate)
            .NotEmpty()
            .WithMessage("Contribution date is required.");

        RuleFor(x => x.MemberId)
            .GreaterThan(0)
            .WithMessage("Valid MemberId is required.");
    }
}
