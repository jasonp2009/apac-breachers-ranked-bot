using ApacBreachersRanked.Application.Stats.Models;
using ApacBreachersRanked.Domain.Match.Entities;
using AutoMapper;

namespace ApacBreachersRanked.Application.Stats.MappingProfiles;

public class MatchPlayerDtoProfile : Profile
{
    public MatchPlayerDtoProfile()
    {
        CreateMap<MatchPlayer, MatchPlayerDto>();
    }
}
