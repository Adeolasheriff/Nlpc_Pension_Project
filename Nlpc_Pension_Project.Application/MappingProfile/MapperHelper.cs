

using AutoMapper;

namespace Nlpc_Pension_Project.Application.MappingProfile;

public static class MapperHelper
{
    public static IMapper GetMapper()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        });

        return config.CreateMapper();
    }
}
