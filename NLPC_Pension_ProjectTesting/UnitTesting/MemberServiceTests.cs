using AutoMapper;
using Moq;
using Nlpc_Pension_Project.Application.Dtos;
using Nlpc_Pension_Project.Application.MappingProfile;
using Nlpc_Pension_Project.Application.Services;
using Nlpc_Pension_Project.Domain.Entities;
using Nlpc_Pension_Project.Infrastructure;

namespace NLPC_Pension_ProjectTesting.UnitTesting;

// MemberServiceTests.cs

    public class MemberServiceTests
    {
        private readonly IMapper _mapper;

        private readonly Mock<IRepository<Member>> _mockRepo;

        public MemberServiceTests()
        {
            _mapper = MapperHelper.GetMapper();
            _mockRepo = new Mock<IRepository<Member>>();
        }

        //[Fact]
        //public async Task GetAllAsync_ReturnsAllMembers()
        //{
        //    // Arrange
        //    var members = new List<Member>
        //    {
        //        new Member { Id = 1, FirstName = "John", LastName = "Doe" },
        //        new Member { Id = 2, FirstName = "Jane", LastName = "Smith" }
        //    };

        //    _mockRepo.Setup(r => r.ListAllMemberAsync()).ReturnsAsync(members);

        //    var service = new MemberService(_mockRepo.Object, _mapper);

        //    // Act
        //    var result = await service.GetAllAsync();

        //    // Assert
        //    Assert.True(result.IsSuccess);
        //    Assert.Equal(2, result.Content.Count());
        //    Assert.Equal("Members retrieved successfully", result.Value);
        //}

        //[Fact]
        //public async Task GetByIdAsync_MemberExists_ReturnsSuccess()
        //{
        //    // Arrange
        //    var member = new Member { Id = 1, FirstName = "John", LastName = "Doe" };
        //    _mockRepo.Setup(r => r.GetMemberByIdAsync(1)).ReturnsAsync(member);

        //    var service = new MemberService(_mockRepo.Object, _mapper);

        //    // Act
        //    var result = await service.GetByIdAsync(1);

        //    // Assert
        //    Assert.True(result.IsSuccess);
        //    Assert.Equal("John", result.Content.FirstName);
        //}

        [Fact]
        public async Task GetByIdAsync_MemberNotFound_Returns404()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetMemberByIdAsync(999)).ReturnsAsync((Member)null);

            var service = new MemberService(_mockRepo.Object, _mapper);

            // Act
            var result = await service.GetByIdAsync(999);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Member not found", result.ErrorMessage);
        }

        //[Fact]
        //public async Task CreateAsync_ValidMember_ReturnsCreatedMember()
        //{
        //    // Arrange
        //    var createDto = new CreateMemberDto
        //    {
        //        FirstName = "Alice",
        //        LastName = "Johnson",
        //        Email = "alice@example.com"
        //    };

        //    var expectedMember = new Member
        //    {
        //        Id = 1,
        //        FirstName = "Alice",
        //        LastName = "Johnson",
        //        Email = "alice@example.com"
        //    };

        //    _mockRepo.Setup(r => r.AddAsync(It.IsAny<Member>())).ReturnsAsync(expectedMember);

        //    var service = new MemberService(_mockRepo.Object, _mapper);

        //    // Act
        //    var result = await service.CreateAsync(createDto);

        //    // Assert
        //    Assert.True(result.IsSuccess);
        //    Assert.Equal("Alice", result.Content.FirstName);
        //    Assert.Equal("Member created", result.Message);
        //}

        [Fact]
        public async Task UpdateAsync_ExistingMember_ReturnsSuccess()
        {
            // Arrange
            var existingMember = new Member
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com"
            };

            var updateDto = new CreateMemberDto
            {
                FirstName = "John",
                LastName = "Updated",
                Email = "updated@example.com"
            };

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingMember);
            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Member>())).Returns(Task.CompletedTask);

            var service = new MemberService(_mockRepo.Object, _mapper);

            // Act
            var result = await service.UpdateAsync(1, updateDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Member updated successfully", result.Message);
        }

        [Fact]
        public async Task UpdateAsync_MemberNotFound_Returns404()
        {
            // Arrange
            var updateDto = new CreateMemberDto
            {
                FirstName = "Invalid",
                LastName = "Member"
            };

            _mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Member)null);

            var service = new MemberService(_mockRepo.Object, _mapper);

            // Act
            var result = await service.UpdateAsync(999, updateDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Member not found", result.ErrorMessage);
        }

        [Fact]
        public async Task DeleteAsync_MemberExists_ReturnsSuccess()
        {
            // Arrange
            var member = new Member { Id = 1 };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(member);
            _mockRepo.Setup(r => r.DeleteAsync(member)).Returns(Task.CompletedTask);

            var service = new MemberService(_mockRepo.Object, _mapper);

            // Act
            var result = await service.DeleteAsync(1);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Member deleted successfully", result.Message);
        }

        [Fact]
        public async Task DeleteAsync_MemberNotFound_Returns404()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Member)null);

            var service = new MemberService(_mockRepo.Object, _mapper);

            // Act
            var result = await service.DeleteAsync(999);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Member not found", result.ErrorMessage);
        }
    }
