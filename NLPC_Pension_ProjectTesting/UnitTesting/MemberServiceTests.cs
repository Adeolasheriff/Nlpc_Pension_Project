using Xunit;
using Moq;
using AutoMapper;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Nlpc_Pension_Project.Application.Services;
using Nlpc_Pension_Project.Application.Services.Interface;
using Nlpc_Pension_Project.Application.Dtos;
using Nlpc_Pension_Project.Domain.Entities;
using Nlpc_Pension_Project.Infrastructure.Repository;

public class MemberServiceTests
{
    private readonly Mock<IRepository<Member>> _mockRepo;
    private readonly Mock<IMapper> _mockMapper;
    
    private readonly MemberService _memberService;

    public MemberServiceTests()
    {
        _mockRepo = new Mock<IRepository<Member>>();
        _mockMapper = new Mock<IMapper>();
        _memberService = new MemberService(_mockRepo.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsMember_WhenExists()
    {
        // Arrange
        var memberId = 1;
        var member = new Member {Id = memberId };
        var dto = new MemberDto { Id = memberId };

        _mockRepo.Setup(r => r.GetMemberByIdAsync(memberId)).ReturnsAsync(member);
        _mockMapper.Setup(m => m.Map<MemberDto>(member)).Returns(dto);

        // Act
        var result = await _memberService.GetByIdAsync(memberId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(dto.Id, result.Value.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNotFound_WhenMemberDoesNotExist()
    {
        _mockRepo.Setup(r => r.GetMemberByIdAsync(It.IsAny<int>())).ReturnsAsync((Member)null);

        var result = await _memberService.GetByIdAsync(99);

        Assert.False(result.IsSuccess);
        Assert.Equal("Member not found", result.ErrorMessage);
    }

    [Fact]
    public async Task CreateAsync_ReturnsCreatedMember()
    {
        var dto = new CreateMemberDto { FirstName = "John" };
        var member = new Member { Id = 1, FirstName = "John" };
        var mappedDto = new MemberDto { Id = 1, FirstName = "John" };

        _mockMapper.Setup(m => m.Map<Member>(dto)).Returns(member);
        _mockRepo.Setup(r => r.AddAsync(member)).ReturnsAsync(member);
        _mockMapper.Setup(m => m.Map<MemberDto>(member)).Returns(mappedDto);

        var result = await _memberService.CreateAsync(dto);

        Assert.True(result.IsSuccess);
        Assert.Equal("John", result.Value.FirstName);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllMembers()
    {
        var members = new List<Member> { new Member { Id = 1 }, new Member { Id = 2 } };
        var dtos = new List<MemberDto> { new MemberDto { Id = 1 }, new MemberDto { Id = 2 } };

        _mockRepo.Setup(r => r.ListAllMemberAsync()).ReturnsAsync(members);
        _mockMapper.Setup(m => m.Map<IEnumerable<MemberDto>>(members)).Returns(dtos);

        var result = await _memberService.GetAllAsync();

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count());
    }

    [Fact]
    public async Task UpdateAsync_ReturnsTrue_WhenSuccessful()
    {
        var dto = new CreateMemberDto { FirstName = "Jane" };
        var member = new Member { Id = 1 };

        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(member);
        _mockMapper.Setup(m => m.Map(dto, member));
        _mockRepo.Setup(r => r.UpdateAsync(member)).Returns(Task.CompletedTask);

        var result = await _memberService.UpdateAsync(1, dto);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsTrue_WhenSuccessful()
    {
        var member = new Member { Id = 98 };

        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(member);
        _mockRepo.Setup(r => r.DeleteAsync(member)).Returns(Task.CompletedTask);

        var result = await _memberService.DeleteAsync(1);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }
}

