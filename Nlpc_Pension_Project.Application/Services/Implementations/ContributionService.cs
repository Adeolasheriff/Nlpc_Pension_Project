using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Nlpc_Pension_Project.Application.Dtos;
using Nlpc_Pension_Project.Application.Services.Interface;
using Nlpc_Pension_Project.Domain.Entities;
using Nlpc_Pension_Project.Domain.Enums;
using Nlpc_Pension_Project.Infrastructure.Repository;

namespace Nlpc_Pension_Project.Application.Services.Implementations;

public class ContributionService : IContributionService
{
    private readonly IRepository<Contribution> _contributionRepository;
    private readonly IMapper _mapper;
    private readonly IRepository<Member> _memberRepository;


    public ContributionService(
        IRepository<Contribution> contributionRepository,
        IMapper mapper,
        IRepository<Member> memberRepository)
    {
        _contributionRepository = contributionRepository;
        _mapper = mapper;
        _memberRepository = memberRepository;
    }

    public async Task<Responses<ContributionProcessingDto>> CalculateContribution(ContributionProcessingDto request, CancellationToken ct)
    {
        var requestTime = DateTime.UtcNow;
        var requestId = Guid.NewGuid().ToString();

        try
        {
            // Validate member exists
            var member = await _memberRepository.GetByIdAsync(request.MemberId);
            if (member == null || member.IsDeleted)
            {
                return Responses<ContributionProcessingDto>.Failure("Member not found");
            }


            // Business rule: Monthly contributions must be within the same calendar month
            if (request.Type == ContributionType.Monthly)
            {
                var currentMonthContributions = await _contributionRepository.ListAllAsync();

                bool alreadyContributedThisMonth = currentMonthContributions
                    .Any(c => c.Type == ContributionType.Monthly &&
                              c.ContributionDate.Year == request.ContributionDate.Year &&
                              c.ContributionDate.Month == request.ContributionDate.Month &&
                              c.MemberId == request.MemberId);

                if (alreadyContributedThisMonth)
                {
                    return Responses<ContributionProcessingDto>.Failure(requestTime,
                        "A monthly contribution already exists for this member in the selected month.",
                        // Conflict
                        requestId);
                }
            }

            var contribution = new Contribution
            {
                ReferenceNumber = Guid.NewGuid(),
                Type = request.Type,
                Amount = request.Amount,
                ContributionDate = request.ContributionDate,
                MemberId = request.MemberId
            };

            var createdContribution = await _contributionRepository.AddAsync(contribution);

            return Responses<ContributionProcessingDto>.Success(requestTime, request, "201", "Contribution recorded successfully", null, requestId);
        }
        catch (ValidationException ex)
        {
            return Responses<ContributionProcessingDto>.Failure($"Validation error: {ex.Message}");
        }
        catch (Exception ex)
        {
            return Responses<ContributionProcessingDto>.Failure($"An unexpected error occurred{ex.Message}");
        }
    }

    // Generate member contribution statement
    public async Task<Responses<IEnumerable<ContributionDto>>> GetContributionStatement(int memberId, CancellationToken ct)
    {
        var requestTime = DateTime.UtcNow;
        var requestId = Guid.NewGuid().ToString();

        try
        {
            // Fetch member with contributions
            var member = await _memberRepository
                .ListAllMemberAsync(); // this returns members with included contributions

            var targetMember = member.FirstOrDefault(m => m.Id == memberId && !m.IsDeleted);

            if (targetMember == null)
            {
                return Responses<IEnumerable<ContributionDto>>.Failure(requestTime, "Member not found", "404");
            }

            var contributions = targetMember.Contributions
                .Where(c => !c.IsDeleted)
                .OrderByDescending(c => c.ContributionDate)
                .ToList();
            

            var dto = _mapper.Map<IEnumerable<ContributionDto>>(contributions);

            return Responses<IEnumerable<ContributionDto>>.Success(requestTime, dto, "200", "Contribution statement generated", null, requestId);
        }
        catch (Exception ex)
        {
            return Responses<IEnumerable<ContributionDto>>.Failure(requestTime, $"An error occurred: {ex.Message}", "500");
        }
    }



}
