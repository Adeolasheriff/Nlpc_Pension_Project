using Microsoft.EntityFrameworkCore;
using Nlpc_Pension_Project.Application.Dtos;
using Nlpc_Pension_Project.Domain.Entities;
using Nlpc_Pension_Project.Domain.Enums;
using Nlpc_Pension_Project.Infrastructure;

namespace Nlpc_Pension_Project.Application.Services;

public class CalculateBenefitService : ICalculateBenefit
{
    private readonly ApplicationDbContext _context;

    public CalculateBenefitService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Responses<BenefitDto>> CalculateBenefit(int memberId, CancellationToken ct)
    {
        var requestTime = DateTime.UtcNow;
        var requestId = Guid.NewGuid().ToString();

        try
        {
            var member = await _context.Members
                .Include(m => m.Contributions)
                .FirstOrDefaultAsync(m => m.Id == memberId && !m.IsDeleted, ct);

            if (member == null)
            {
                return Responses<BenefitDto>.Failure(requestTime, "Member not found", "404");
            }

            // Minimum contribution period before benefit eligibility
            const int minContributionMonths = 60;

            var totalContributionMonths = member.Contributions
                .Select(c => new DateTime(c.ContributionDate.Year, c.ContributionDate.Month, 1))
                .Distinct()
                .Count();

            var eligible = totalContributionMonths >= minContributionMonths;

            var totalContributions = member.Contributions.Sum(c => c.Amount);
            var benefitAmount = eligible ? totalContributions * 0.015m : 0;

            var result = new BenefitDto
            {
                Type = BenefitType.Retirement,
                CalculationDate = DateTime.UtcNow,
                EligibilityStatus = eligible,
                Amount = benefitAmount,
                MemberId = member.Id
            };

            return Responses<BenefitDto>.Success(requestTime, result, "200", "Benefit calculated successfully", null, requestId);
        }
        catch (Exception ex)
        {
            return Responses<BenefitDto>.Failure(requestTime, $"An unexpected error occurred: {ex.Message}", "500");
        }
    }
}
