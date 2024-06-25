using ApacBreachersRanked.Domain.Common;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.Match.Enums;

namespace ApacBreachersRanked.Domain.MatchData.Entities;

public class GameDataEntity : BaseEntity
{
    public Guid MatchId { get; init; }
    public MatchEntity Match { get; init; }
    public MapScore Score { get; init; }
    public IEnumerable<GamePlayerData> Players { get; init; }
    public IEnumerable<GamePlayerData> HomePlayers => Players.Where(player => player.Side == MatchSide.Home);
    public IEnumerable<GamePlayerData> AwayPlayers => Players.Where(player => player.Side == MatchSide.Away);
}