

using AutoMapper;
using Nlpc_Pension_Project.Application.Dtos;
using Nlpc_Pension_Project.Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Nlpc_Pension_Project.Application.MappingProfile;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Member, MemberDto>();

        CreateMap<CreateMemberDto, Member>();

        CreateMap<Employer, EmployerDto>();


        CreateMap<CreateEmployerDto, Employer>();

        CreateMap<Contribution, ContributionDto>();



    }
}
