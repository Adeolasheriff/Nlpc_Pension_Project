using AutoMapper;
using Azure;
using Nlpc_Pension_Project.Application.Dtos;
using Nlpc_Pension_Project.Domain.Entities;
using Nlpc_Pension_Project.Infrastructure;


namespace Nlpc_Pension_Project.Application.Services;

public class MemberService : IMemberService
{
    private readonly IRepository<Member> _repository;
    private readonly IMapper _mapper;

    public MemberService(IRepository<Member> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    public async Task<Responses<IEnumerable<MemberDto>>> GetAllAsync()
    {
        var requestTime = DateTime.UtcNow;
        var requestId = Guid.NewGuid().ToString();

        try
        {
            var members = await _repository.ListAllMemberAsync();

            // Sort contributions before mapping
            foreach (var member in members)
            {
                member.Contributions = member.Contributions
                    .Where(c => !c.IsDeleted)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToList();
            }

            var dto = _mapper.Map<IEnumerable<MemberDto>>(members);


            return Responses<IEnumerable<MemberDto>>.Success(
                requestTime, dto, "200", "Members retrieved successfully", null, requestId
            );
        }
        catch (Exception ex)
        {
            return Responses<IEnumerable<MemberDto>>.Failure(
                requestTime, $"An error occurred: {ex.Message}", "500"
            );
        }
    }

    //public async Task<Responses<IEnumerable<MemberDto>>> GetAllAsync()
    //{
    //    var requestTime = DateTime.UtcNow;
    //    var requestId = Guid.NewGuid().ToString();
    //    try
    //    {
    //        var members = await _repository.ListAllMemberAsync();
    //        var dto = _mapper.Map<IEnumerable<MemberDto>>(members);
    //        return Responses<IEnumerable<MemberDto>>.Success(requestTime, dto, "200", "Members retrieved successfully", null, requestId);
    //    }
    //    catch (Exception ex)
    //    {
    //        return Responses<IEnumerable<MemberDto>>.Failure(requestTime, $"An error occurred: {ex.Message}", "500");
    //    }
    //}

    public async Task<Responses<MemberDto>> GetByIdAsync(int id)
    {
        var requestTime = DateTime.UtcNow;
        var requestId = Guid.NewGuid().ToString();
        try
        {
            var member = await _repository.GetMemberByIdAsync(id);
            if (member == null)
                return Responses<MemberDto>.Failure(requestTime, "Member not found", "404");

            var dto = _mapper.Map<MemberDto>(member);
            return Responses<MemberDto>.Success(requestTime, dto, "200", "Member found", null, requestId);
        }
        catch (Exception ex)
        {
            return Responses<MemberDto>.Failure(requestTime, $"An error occurred: {ex.Message}", "500");
        }
    }

    public async Task<Responses<MemberDto>> CreateAsync(CreateMemberDto dto)
    {
        var requestTime = DateTime.UtcNow;
        var requestId = Guid.NewGuid().ToString();
        try
        {
            var member = _mapper.Map<Member>(dto);
            var created = await _repository.AddAsync(member);
            var result = _mapper.Map<MemberDto>(created);
            return Responses<MemberDto>.Success(requestTime, result, "201", "Member created", null, requestId);
        }
        catch (Exception ex)
        {
            return Responses<MemberDto>.Failure(requestTime, $"An error occurred: {ex.Message}", "500");
        }
    }

    public async Task<Responses<bool>> UpdateAsync(int id, CreateMemberDto dto)
    {
        var requestTime = DateTime.UtcNow;
        var requestId = Guid.NewGuid().ToString();
        try
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                return new Responses<bool>
                {
                    Content = false,
                    ErrorMessage = "Member not found",
                    RequestId = requestId,
                    RequestTime = requestTime,
                    ResponseTime = DateTime.UtcNow,
                    IsSuccess = false
                };

            _mapper.Map(dto, existing);
            await _repository.UpdateAsync(existing);

            return Responses<bool>.Success(requestTime, true, "200", "Member updated successfully", null, requestId);
        }
        catch (Exception ex)
        {
            return Responses<bool>.Failure(requestTime, $"An error occurred: {ex.Message}", "500");
        }
    }

    public async Task<Responses<bool>> DeleteAsync(int id)
    {
        var requestTime = DateTime.UtcNow;
        var requestId = Guid.NewGuid().ToString();
        try
        {
            var member = await _repository.GetByIdAsync(id);
            if (member == null)
                return new Responses<bool>
                {
                    Content = false,
                    ErrorMessage = "Member not found",
                    RequestId = requestId,
                    RequestTime = requestTime,
                    ResponseTime = DateTime.UtcNow,
                    IsSuccess = false
                };

            await _repository.DeleteAsync(member);
            return Responses<bool>.Success(requestTime, true, "200", "Member deleted successfully", null, requestId);
        }
        catch (Exception ex)
        {
            return Responses<bool>.Failure(requestTime, $"An error occurred: {ex.Message}", "500");
        }
    }
}
