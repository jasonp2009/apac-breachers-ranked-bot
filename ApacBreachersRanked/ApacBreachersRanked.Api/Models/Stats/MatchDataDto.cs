using ApacBreachersRanked.Infrastructure.Breachers.Entities;

namespace ApacBreachersRanked.Api.Models.Stats;

public class MatchDataDto
{
    public MatchDto Match { get; set; }
    public BreachersMatchDataEntity MatchData { get; set; }
}
