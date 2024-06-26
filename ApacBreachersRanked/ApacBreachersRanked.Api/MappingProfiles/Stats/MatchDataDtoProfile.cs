using ApacBreachersRanked.Api.Models.Stats;
using ApacBreachersRanked.Application.Match.Queries;
using AutoMapper;

namespace ApacBreachersRanked.Api.MappingProfiles.Stats;

public class MatchDataDtoProfile : Profile
{
    public MatchDataDtoProfile()
    {
        CreateMap<GetMatchDataResponse, MatchDataDto>()
            .ForMember(dest => dest.Match,
                opt => opt.MapFrom(src => src.Match))
            .ForMember(dest => dest.GameData,
                opt => opt.MapFrom(src => src.GameData));
    }
}
