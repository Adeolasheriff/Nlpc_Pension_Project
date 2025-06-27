using Nlpc_Pension_Project.Application.Dtos;

namespace Nlpc_Pension_Project.Application.Services.Interface;

public interface IMemberService
{
    Task<Responses<IEnumerable<MemberDto>>> GetAllAsync();
    Task<Responses<MemberDto>> GetByIdAsync(int id);
    Task<Responses<MemberDto>> CreateAsync(CreateMemberDto dto);
    Task<Responses<bool>> UpdateAsync(int id, CreateMemberDto dto);
    Task<Responses<bool>> DeleteAsync(int id);
}

