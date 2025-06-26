

using Nlpc_Pension_Project.Domain.Enums;

namespace Nlpc_Pension_Project.Application.Dtos;

public class ContributionDto
{
    public int Id { get; set; }
    public Guid ReferenceNumber { get; set; }
    public ContributionType Type { get; set; }
    public decimal Amount { get; set; }
    public DateTime ContributionDate { get; set; }
    public int MemberId { get; set; }
}
