

using AutoMapper;
using Nlpc_Pension_Project.Application.Dtos;
using Nlpc_Pension_Project.Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Nlpc_Pension_Project.Application.MappingProfile;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //CreateMap<Member, MemberDto>();
        CreateMap<Member, MemberDto>()
       .AfterMap((src, dest) =>
    {
        dest.Contributions = src.Contributions
            .Where(c => !c.IsDeleted)
            .OrderByDescending(c => c.CreatedAt)
            .ToList();
    });

        CreateMap<CreateMemberDto, Member>();
    }
}
