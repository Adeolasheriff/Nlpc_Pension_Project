using AutoMapper;
using Nlpc_Pension_Project.Application.Dtos;
using Nlpc_Pension_Project.Application.Services.Interface;
using Nlpc_Pension_Project.Domain.Entities;
using Nlpc_Pension_Project.Infrastructure.Repository;

namespace Nlpc_Pension_Project.Application.Services.Implementations;

public class EmployerService : IEmployerService
{
    private readonly IRepository<Employer> _repository;
    private readonly IMapper _mapper;

    public EmployerService(IRepository<Employer> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Responses<IEnumerable<EmployerDto>>> GetAllAsync()
    {
        var requestTime = DateTime.UtcNow;
        var requestId = Guid.NewGuid().ToString();

        try
        {
            var employers = await _repository.ListAllAsync();
            var dto = _mapper.Map<IEnumerable<EmployerDto>>(employers);

            return Responses<IEnumerable<EmployerDto>>.Success(
                requestTime, dto, "200", "Employers retrieved successfully", null, requestId
            );
        }
        catch (Exception ex)
        {
            return Responses<IEnumerable<EmployerDto>>.Failure(
                requestTime, $"An error occurred: {ex.Message}", "500"
            );
        }
    }

    public async Task<Responses<EmployerDto>> CreateAsync(CreateEmployerDto employerDto)
    {
        var requestTime = DateTime.UtcNow;
        var requestId = Guid.NewGuid().ToString();

        try
        {
            // 🔍 Check if employer with same RegistrationNumber already exists
            //var existing = await _repository.(
            //    e => e.RegistrationNumber == employerDto.RegistrationNumber && !e.IsDeleted
            //);

            var existing = await _repository.ListAllAsync();
            if (existing != null && existing.Any(e => e.RegistrationNumber == employerDto.RegistrationNumber && !e.IsDeleted))
            {
                return Responses<EmployerDto>.Failure(
                    requestTime, "Employer with this registration number already exists.", "409"
                );
            }

            var employer = _mapper.Map<Employer>(employerDto);
            await _repository.AddAsync(employer);

            var resultDto = _mapper.Map<EmployerDto>(employer);

            return Responses<EmployerDto>.Success(
                requestTime, resultDto, "201", "Employer created successfully", null, requestId
            );
        }
        catch (Exception ex)
        {
            return Responses<EmployerDto>.Failure(
                requestTime, $"An error occurred: {ex.Message}", "500"
            );
        }
    }
}

