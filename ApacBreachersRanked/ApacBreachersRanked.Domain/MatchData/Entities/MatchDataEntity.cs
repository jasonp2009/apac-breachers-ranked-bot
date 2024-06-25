using ApacBreachersRanked.Domain.Common;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.Match.Enums;

namespace ApacBreachersRanked.Domain.MatchData.Models;

public class MatchDataEntity : BaseEntity
{
    public Guid MatchId { get; init; }
    public MatchEntity Match { get; init; }
    public Map Map { get; init; }

    public MatchScore Score { get; init; }
    public IEnumerable<MatchPlayerData> Players { get; init; }
}