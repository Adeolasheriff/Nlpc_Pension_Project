

namespace NLPC_Pension_ProjectTesting.UnitTesting;

using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nlpc_Pension_Project.Domain.Entities;
using Nlpc_Pension_Project.Domain.Enums;
using Nlpc_Pension_Project.Application.Services.Implementations;
using Nlpc_Pension_Project.Application.Dtos;
using Nlpc_Pension_Project.Infrastructure.AppDbContext;

public class CalculateBenefitServiceTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly CalculateBenefitService _benefitService;

    public CalculateBenefitServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _benefitService = new CalculateBenefitService(_dbContext);
    }

    [Fact]
    public async Task CalculateBenefit_ReturnsBenefit_WhenEligible()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;

        using var context = new ApplicationDbContext(options);

        var member = new Member
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            PhoneNumber = "1234567890",
            Contributions = Enumerable.Range(1, 60).Select(i => new Contribution
            {
                ContributionDate = DateTime.UtcNow.AddMonths(-i),
                Amount = 1000,
                Type = ContributionType.Monthly,
                MemberId = 1
            }).ToList()
        };

        context.Members.Add(member);
        await context.SaveChangesAsync();

        var service = new CalculateBenefitService(context);

        // Act
        var result = await service.CalculateBenefit(1, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value.EligibilityStatus);
        Assert.True(result.Value.Amount > 0);
        Assert.Equal(member.Id, result.Value.MemberId);
    }


    [Fact]
    public async Task CalculateBenefit_ReturnsZero_WhenNotEligible()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;

        using var context = new ApplicationDbContext(options);

        var member = new Member
        {
            Id = 2,
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@example.com",
            PhoneNumber = "0987654321",
            Contributions = Enumerable.Range(1, 30).Select(i => new Contribution
            {
                ContributionDate = DateTime.UtcNow.AddMonths(-i),
                Amount = 1000,
                Type = ContributionType.Monthly,
                MemberId = 2
            }).ToList()
        };

        context.Members.Add(member);
        await context.SaveChangesAsync();

        var service = new CalculateBenefitService(context);

        // Act
        var result = await service.CalculateBenefit(2, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.Value.EligibilityStatus); // Not eligible
        Assert.Equal(0, result.Value.Amount);         // Benefit should be 0
        Assert.Equal(member.Id, result.Value.MemberId);
    }


    [Fact]
    public async Task CalculateBenefit_ReturnsNotFound_WhenMemberDoesNotExist()
    {
        // Act
        var result = await _benefitService.CalculateBenefit(999, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Member not found", result.ErrorMessage);
    }
}
