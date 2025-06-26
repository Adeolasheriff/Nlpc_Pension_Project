namespace Nlpc_Pension_Project.Domain.Entities;

public class Employer : BaseEntity
{
    public string CompanyName { get; set; }
    public string RegistrationNumber { get; set; }
    public bool IsActive { get; set; } = true;
}
