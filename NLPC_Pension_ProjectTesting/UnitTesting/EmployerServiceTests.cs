namespace NLPC_Pension_ProjectTesting.UnitTesting;

using Xunit;
using Moq;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nlpc_Pension_Project.Application.Services;
using Nlpc_Pension_Project.Application.Services.Interface;
using Nlpc_Pension_Project.Domain.Entities;
using Nlpc_Pension_Project.Application.Dtos;
using Nlpc_Pension_Project.Infrastructure.Repository;
using Nlpc_Pension_Project.Application.Services.Implementations;

public class EmployerServiceTests
{
    private readonly Mock<IRepository<Employer>> _mockRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly EmployerService _service;

    public EmployerServiceTests()
    {
        _mockRepo = new Mock<IRepository<Employer>>();
        _mockMapper = new Mock<IMapper>();
        _service = new EmployerService(_mockRepo.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task CreateAsync_ReturnsCreatedEmployerDto_WhenSuccessful()
    {
        // Arrange
        var dto = new CreateEmployerDto { CompanyName = "Test Corp" };
        var entity = new Employer { Id = 1, CompanyName = "Test Corp" };
        var resultDto = new EmployerDto { Id = 1, CompanyName = "Test Corp" };

        _mockMapper.Setup(m => m.Map<Employer>(dto)).Returns(entity);
        _mockRepo.Setup(r => r.AddAsync(entity)).ReturnsAsync(entity);
        _mockMapper.Setup(m => m.Map<EmployerDto>(entity)).Returns(resultDto);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Test Corp", result.Value.CompanyName);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsListOfEmployerDtos()
    {
        // Arrange
        var employers = new List<Employer>
        {
            new Employer { Id = 1, CompanyName = "Company A" },
            new Employer { Id = 2, CompanyName = "Company B" }
        };

        var expectedDtos = new List<EmployerDto>
        {
            new EmployerDto { Id = 1, CompanyName = "Company A" },
            new EmployerDto { Id = 2, CompanyName = "Company B" }
        };

        _mockRepo.Setup(r => r.ListAllAsync()).ReturnsAsync(employers);
        _mockMapper.Setup(m => m.Map<IEnumerable<EmployerDto>>(employers)).Returns(expectedDtos);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count());
    }
}
