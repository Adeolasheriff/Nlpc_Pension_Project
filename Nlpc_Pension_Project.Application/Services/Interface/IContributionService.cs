using Nlpc_Pension_Project.Application.Dtos;

namespace Nlpc_Pension_Project.Application.Services.Interface;

public interface IContributionService
{
    Task<Responses<ContributionProcessingDto>> CalculateContribution(ContributionProcessingDto request, CancellationToken ct);

    // method to generate contribution statement for a member
    Task<Responses<IEnumerable<ContributionDto>>> GetContributionStatement(int memberId, CancellationToken ct);



}
