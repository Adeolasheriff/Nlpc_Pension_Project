using Nlpc_Pension_Project.Application.Dtos;

namespace Nlpc_Pension_Project.Application.Services.Interface;

public interface ICalculateBenefit
{
    Task<Responses<BenefitDto>> CalculateBenefit(int memberId, CancellationToken ct);
}
