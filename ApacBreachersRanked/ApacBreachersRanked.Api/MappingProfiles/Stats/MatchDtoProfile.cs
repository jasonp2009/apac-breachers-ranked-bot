using ApacBreachersRanked.Api.Models.Stats;
using ApacBreachersRanked.Application.Stats.Models;
using ApacBreachersRanked.Domain.Match.Entities;
using AutoMapper;

namespace ApacBreachersRanked.Api.MappingProfiles.Stats;

public class MatchDtoProfile : Profile
{
    public MatchDtoProfile()
    {
        CreateMap<MatchEntity, MatchDto>()
            .ForMember(dest => dest.AllPlayers,
                opt => opt.MapFrom(src => src.AllPlayers))
            .ForMember(dest => dest.HomePlayers, opt => opt.Ignore())
            .ForMember(dest => dest.AwayPlayers, opt => opt.Ignore())
            .ForMember(dest => dest.HostPlayer, opt => opt.Ignore());
    }
}
