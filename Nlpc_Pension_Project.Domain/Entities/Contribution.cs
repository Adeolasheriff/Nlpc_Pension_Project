using Nlpc_Pension_Project.Domain.Enums;
using System.Text.Json.Serialization;

namespace Nlpc_Pension_Project.Domain.Entities;

public class Contribution : BaseEntity
{
    public Guid ReferenceNumber { get; set; }
    public ContributionType Type { get; set; }
    public decimal Amount { get; set; }
    public DateTime ContributionDate { get; set; }

    public int MemberId { get; set; }
    [JsonIgnore]
    public Member Member { get; set; }
}// Prevent cycles here}

