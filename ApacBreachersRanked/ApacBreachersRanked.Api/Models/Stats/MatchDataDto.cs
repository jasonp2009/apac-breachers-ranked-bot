using ApacBreachersRanked.Domain.MatchData.Entities;

namespace ApacBreachersRanked.Api.Models.Stats;

public class MatchDataDto
{
    public MatchDto Match { get; set; }
    public IEnumerable<GameDataEntity> GameData { get; set; }
}
