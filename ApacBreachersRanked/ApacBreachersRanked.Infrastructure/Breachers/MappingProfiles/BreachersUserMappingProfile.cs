using ApacBreachersRanked.Infrastructure.Breachers.Models;
using AutoMapper;

using ApplicationBreachersUser = ApacBreachersRanked.Application.BreachersUsers.Models.BreachersUser;

namespace ApacBreachersRanked.Infrastructure.Breachers.MappingProfiles;

public class BreachersUserMappingProfile : Profile
{
    public BreachersUserMappingProfile()
    {
        CreateMap<BreachersUser, ApplicationBreachersUser>();
    }
}
