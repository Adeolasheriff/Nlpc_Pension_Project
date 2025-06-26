namespace Nlpc_Pension_Project.Domain.Entities;

public class Member : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<Contribution> Contributions { get; set; } = new List<Contribution>();
    public ICollection<Benefit> Benefits { get; set; } = new List<Benefit>();
}
