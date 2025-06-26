

using Nlpc_Pension_Project.Domain.Enums;

namespace Nlpc_Pension_Project.Application.Dtos;

public class ContributionProcessingDto
{
    
    public ContributionType Type { get; set; }
    public decimal Amount { get; set; }
    public DateTime ContributionDate { get; set; }
    public int MemberId { get; set; }

}
