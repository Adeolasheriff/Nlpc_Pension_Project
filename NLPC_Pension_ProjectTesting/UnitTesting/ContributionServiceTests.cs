

namespace NLPC_Pension_ProjectTesting.UnitTesting;

using Xunit;
using Moq;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nlpc_Pension_Project.Application.Dtos;
using Nlpc_Pension_Project.Application.Services;
using Nlpc_Pension_Project.Application.Services.Implementations;
using Nlpc_Pension_Project.Domain.Entities;
using Nlpc_Pension_Project.Domain.Enums;
using Nlpc_Pension_Project.Infrastructure.Repository;

public class ContributionServiceTests
{
    private readonly Mock<IRepository<Contribution>> _mockContributionRepo;
    private readonly Mock<IRepository<Member>> _mockMemberRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly ContributionService _service;

    public ContributionServiceTests()
    {
        _mockContributionRepo = new Mock<IRepository<Contribution>>();
        _mockMemberRepo = new Mock<IRepository<Member>>();
        _mockMapper = new Mock<IMapper>();

        _service = new ContributionService(
            _mockContributionRepo.Object,
            _mockMapper.Object,
            _mockMemberRepo.Object
        );
    }

    [Fact]
    public async Task CalculateContribution_Fails_WhenMemberNotFound()
    {
        var dto = new ContributionProcessingDto { MemberId = 999 };

        _mockMemberRepo.Setup(r => r.GetByIdAsync(dto.MemberId)).ReturnsAsync((Member)null);

        var result = await _service.CalculateContribution(dto, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Member not found", result.ErrorMessage);
    }

    [Fact]
    public async Task CalculateContribution_Fails_IfMonthlyAlreadyExists()
    {
        var dto = new ContributionProcessingDto
        {
            Type = ContributionType.Monthly,
            Amount = 1000,
            ContributionDate = new DateTime(2025, 6, 1),
            MemberId = 1
        };

        _mockMemberRepo.Setup(r => r.GetByIdAsync(dto.MemberId))
            .ReturnsAsync(new Member { Id = 1 });

        _mockContributionRepo.Setup(r => r.ListAllAsync())
            .ReturnsAsync(new List<Contribution>
            {
                new Contribution
                {
                    Type = ContributionType.Monthly,
                    ContributionDate = new DateTime(2025, 6, 15),
                    MemberId = 1
                }
            });

        var result = await _service.CalculateContribution(dto, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("already exists", result.ErrorMessage);
    }

    [Fact]
    public async Task CalculateContribution_Succeeds_ForValidInput()
    {
        var dto = new ContributionProcessingDto
        {
            Type = ContributionType.Voluntary,
            Amount = 1500,
            ContributionDate = DateTime.UtcNow,
            MemberId = 1
        };

        _mockMemberRepo.Setup(r => r.GetByIdAsync(dto.MemberId))
            .ReturnsAsync(new Member { Id = 1 });

        _mockContributionRepo.Setup(r => r.ListAllAsync())
            .ReturnsAsync(new List<Contribution>()); // No conflict

        _mockContributionRepo.Setup(r => r.AddAsync(It.IsAny<Contribution>()))
            .ReturnsAsync(new Contribution());

        var result = await _service.CalculateContribution(dto, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Contribution recorded successfully", result.Message);
    }

    [Fact]
    public async Task GetContributionStatement_Fails_WhenMemberNotFound()
    {
        _mockMemberRepo.Setup(r => r.ListAllMemberAsync())
            .ReturnsAsync(new List<Member>());

        var result = await _service.GetContributionStatement(1, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Member not found", result.ErrorMessage);
    }

    [Fact]
    public async Task GetContributionStatement_Succeeds_WhenMemberExists()
    {
        var member = new Member
        {
            Id = 1,
            Contributions = new List<Contribution>
            {
                new Contribution
                {
                    Id = 1,
                    ContributionDate = new DateTime(2025, 5, 1),
                    IsDeleted = false
                },
                new Contribution
                {
                    Id = 2,
                    ContributionDate = new DateTime(2025, 6, 1),
                    IsDeleted = false
                }
            }
        };

        var expectedDto = new List<ContributionDto>
        {
            new ContributionDto { Id = 2 },
            new ContributionDto { Id = 1 }
        };

        _mockMemberRepo.Setup(r => r.ListAllMemberAsync())
            .ReturnsAsync(new List<Member> { member });

        _mockMapper.Setup(m => m.Map<IEnumerable<ContributionDto>>(It.IsAny<IEnumerable<Contribution>>()))
            .Returns(expectedDto);

        var result = await _service.GetContributionStatement(1, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count());
        Assert.Equal(2, result.Value.First().Id); // Ordered descending
    }
}
