

using Nlpc_Pension_Project.Domain.Enums;
using System.Text.Json.Serialization;

namespace Nlpc_Pension_Project.Domain.Entities;

public class Benefit : BaseEntity
{
    public BenefitType Type { get; set; }
    public DateTime CalculationDate { get; set; }
    public bool EligibilityStatus { get; set; }
    public decimal Amount { get; set; }

    public int MemberId { get; set; }
     [JsonIgnore]
    public Member Member { get; set; } // Prevent cycles here
}