using Nlpc_Pension_Project.Application.Dtos;

namespace Nlpc_Pension_Project.Application.Services.Interface;

public interface IEmployerService
{
    Task<Responses<IEnumerable<EmployerDto>>> GetAllAsync();
    Task<Responses<EmployerDto>> CreateAsync(CreateEmployerDto employerDto);
    
}

