

using Nlpc_Pension_Project.Domain.Enums;

namespace Nlpc_Pension_Project.Application.Dtos;

public class BenefitDto
{
    public int Id { get; set; }
    public BenefitType Type { get; set; }
    public DateTime CalculationDate { get; set; }
    public bool EligibilityStatus { get; set; }
    public decimal Amount { get; set; }
    public int MemberId { get; set; }
}
