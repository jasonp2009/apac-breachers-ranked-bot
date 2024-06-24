using ApacBreachersRanked.Api.Models.Stats;
using ApacBreachersRanked.Infrastructure.Breachers.Queries;
using AutoMapper;

namespace ApacBreachersRanked.Api.MappingProfiles.Stats;

public class MatchDataProfile : Profile
{
    public MatchDataProfile()
    {
        CreateMap<GetMatchDataResponse, MatchDataDto>();
    }
}
