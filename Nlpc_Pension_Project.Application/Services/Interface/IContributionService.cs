using Nlpc_Pension_Project.Application.Dtos;

namespace Nlpc_Pension_Project.Application.Services.Interface;

public interface IContributionService
{
    Task<Responses<ContributionProcessingDto>> CalculateContribution(ContributionProcessingDto request, CancellationToken ct);
}
