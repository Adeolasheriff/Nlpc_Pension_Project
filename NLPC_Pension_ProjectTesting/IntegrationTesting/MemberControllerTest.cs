







////using System.Net;
////using System.Net.Http.Json;
////using Microsoft.AspNetCore.Mvc.Testing;
////using Xunit;
////using Nlpc_Pension_Project.Application.Dtos;
////using Nlpc_Pension_Project.Domain.Entities;
////using global::Nlpc_Pension_Project.Infrastructure;
////using Microsoft.VisualStudio.TestPlatform.TestHost;
////using Nlpc_Pension_Project.Application.Services;
////using CsvHelper;

////namespace NLPC_Pension_ProjectTesting.IntegrationTesting;

////public class MembersControllerRealDbTests
////    : IClassFixture<RealDatabaseWebApplicationFactory<Program>>
////{
////    private readonly HttpClient _client;

////    public MembersControllerRealDbTests(RealDatabaseWebApplicationFactory<Program> factory)
////    {
////        _client = factory.CreateClient();
////    }

////    [Fact]
////    public async Task GetAll_ReturnsAllMembersFromRealDatabase()
////    {
////        // Arrange - seed data directly into DB
////        using var scope = factory.Services.CreateScope();
////        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
////        context.Database.EnsureDeleted(); // optional clean state
////        context.Database.Migrate();

////        context.Members.Add(new Member { FirstName = "John", LastName = "Doe", Email = "john@example.com" });
////        context.Members.Add(new Member { FirstName = "Jane", LastName = "Smith", Email = "jane@example.com" });
////        await context.SaveChangesAsync();

////        // Act
////        var response = await _client.GetAsync("/api/Members");

////        // Assert
////        response.EnsureSuccessStatusCode();
////        var result = await response.Content.ReadFromJsonAsync<Responses<IEnumerable<MemberDto>>>();
////        Assert.NotNull(result);
////        Assert.True(result.IsSuccess);
////        Assert.Equal(2, result.Content.Count());
////    }

////    [Fact]
////    public async Task GetById_ExistingId_ReturnsMember()
////    {
////        // Arrange
////        using var scope = factory.Services.CreateScope();
////        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
////        context.Database.EnsureDeleted();
////        context.Database.Migrate();

////        var member = new Member { FirstName = "Alice", LastName = "Johnson", Email = "alice@example.com" };
////        await context.Members.AddAsync(member);
////        await context.SaveChangesAsync();

////        // Act
////        var response = await _client.GetAsync($"/api/Members/{member.Id}");

////        // Assert
////        response.EnsureSuccessStatusCode();
////        var result = await response.Content.ReadFromJsonAsync<Responses<MemberDto>>();
////        Assert.NotNull(result);
////        Assert.True(result.IsSuccess);
////        Assert.Equal("Alice", result.Value.FirstName);
////    }

////    [Fact]
////    public async Task GetById_NonExistentId_ReturnsNotFound()
////    {
////        // Act
////        var response = await _client.GetAsync("/api/Members/999");

////        // Assert
////        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
////    }

////    [Fact]
////    public async Task Create_ValidMember_ReturnsCreated()
////    {
////        var dto = new CreateMemberDto
////        {
////            FirstName = "Bob",
////            LastName = "Brown",
////            Email = "bob@example.com"
////        };

////        var response = await _client.PostAsJsonAsync("/api/Members", dto);

////        response.EnsureSuccessStatusCode();
////        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

////        var result = await response.Content.ReadFromJsonAsync<Responses<MemberDto>>();
////        Assert.NotNull(result);
////        Assert.True(result.IsSuccess);
////        Assert.Equal("Bob", result.Value.FirstName);
////    }

////    [Fact]
////    public async Task Update_ExistingMember_UpdatesSuccessfully()
////    {
////        // Arrange
////        using var scope = factory.Services.CreateScope();
////        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
////        context.Database.EnsureDeleted();
////        context.Database.Migrate();

////        var member = new Member { FirstName = "OldName", LastName = "Smith", Email = "oldname@example.com" };
////        await context.Members.AddAsync(member);
////        await context.SaveChangesAsync();

////        var updateDto = new CreateMemberDto
////        {
////            FirstName = "UpdatedName",
////            LastName = "Smith",
////            Email = "updated@example.com"
////        };

////        // Act
////        var response = await _client.PutAsJsonAsync($"/api/Members/{member.Id}", updateDto);

////        // Assert
////        response.EnsureSuccessStatusCode();
////        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

////        var updatedMember = await context.Members.FindAsync(member.Id);
////        Assert.NotNull(updatedMember);
////        Assert.Equal("UpdatedName", updatedMember.FirstName);
////    }

////    [Fact]
////    public async Task Delete_ExistingMember_RemovesFromDatabase()
////    {
////        // Arrange
////        using var scope = Factory.Services.CreateScope();
////        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
////        context.Database.EnsureDeleted();
////        context.Database.Migrate();

////        var member = new Member { FirstName = "ToDelete", LastName = "User", Email = "delete@example.com" };
////        await context.Members.AddAsync(member);
////        await context.SaveChangesAsync();

////        // Act
////        var deleteResponse = await _client.DeleteAsync($"/api/Members/{member.Id}");

////        // Assert
////        deleteResponse.EnsureSuccessStatusCode();
////        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

////        var deletedMember = await context.Members.FindAsync(member.Id);
////        Assert.Null(deletedMember); // Or check IsDeleted flag if soft-delete is used
////    }
////}

//using System.Net;
//using System.Net.Http.Json;
//using Xunit;
//using Nlpc_Pension_Project.Application.Dtos;
//using FluentAssertions;
//using Nlpc_Pension_Project.Application.Services;

//namespace Nlpc_Pension_ProjectTesting.IntegrationTests
//{
//    public class MemberIntegrationTests : IClassFixture<CustomWebApplicationFactory>
//    {
//        private readonly HttpClient _client;

//        public MemberIntegrationTests(CustomWebApplicationFactory factory)
//        {
//            _client = factory.CreateClient();
//        }

//        [Fact]
//        public async Task CreateMember_Returns201Created()
//        {
//            // Arrange
//            var dto = new CreateMemberDto
//            {
//                FirstName = "Jane",
//                LastName = "Doe",
//                Email = "jane.doe@email.com",
//                // Add all required fields!
//            };

//            // Act
//            var response = await _client.PostAsJsonAsync("/api/members", dto);

//            // Assert
//            response.StatusCode.Should().Be(HttpStatusCode.Created);

//            var result = await response.Content.ReadFromJsonAsync<Responses<MemberDto>>();
//            result.Should().NotBeNull();
//            result!.HasError.Should().BeFalse();
//            result.Value!.FirstName.Should().Be("Jane");
//        }
//    }
//}
