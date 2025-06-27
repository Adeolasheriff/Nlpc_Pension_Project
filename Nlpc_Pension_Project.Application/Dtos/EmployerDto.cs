
namespace Nlpc_Pension_Project.Application.Dtos;

public class EmployerDto
{
    public int Id { get; set; }
    public string CompanyName { get; set; }
    public string RegistrationNumber { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

