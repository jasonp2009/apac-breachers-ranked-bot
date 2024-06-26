using ApacBreachersRanked.Application.Stats.Models;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.Match.Enums;

namespace ApacBreachersRanked.Api.Models.Stats;

public class MatchDto
{
    public Guid Id { get; set; }
    public int MatchNumber { get; set; }
    public MatchStatus Status { get; set; }
    public IEnumerable<MatchPlayerDto> HomePlayers => AllPlayers.Where(player => player.Side == MatchSide.Home);
    public decimal HomeMMR => HomePlayers.Average(x => x.MMR);
    public IEnumerable<MatchPlayerDto> AwayPlayers => AllPlayers.Where(player => player.Side == MatchSide.Away);
    public decimal AwayMMR => AwayPlayers.Average(x => x.MMR);
    public IEnumerable<MatchPlayerDto> AllPlayers { get; set; }
    public MatchPlayerDto HostPlayer => AllPlayers.FirstOrDefault(player => player.IsHost);
    public MatchScore Score { get; private set; } = null;
}
