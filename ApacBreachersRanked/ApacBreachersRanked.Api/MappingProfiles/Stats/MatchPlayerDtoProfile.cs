using ApacBreachersRanked.Api.Models.Stats;
using ApacBreachersRanked.Application.Stats.Models;
using ApacBreachersRanked.Domain.Match.Entities;
using AutoMapper;

namespace ApacBreachersRanked.Api.MappingProfiles.Stats;

public class MatchPlayerDtoProfile : Profile
{
    public MatchPlayerDtoProfile()
    {
        CreateMap<MatchPlayer, MatchPlayerDto>();
    }
}
