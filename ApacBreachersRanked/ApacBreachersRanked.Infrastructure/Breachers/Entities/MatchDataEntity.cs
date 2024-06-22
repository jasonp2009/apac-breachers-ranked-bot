using ApacBreachersRanked.Domain.Common;
using ApacBreachersRanked.Infrastructure.Breachers.Models;

namespace ApacBreachersRanked.Infrastructure.Breachers.Entities;

public class MatchDataEntity : BaseEntity
{
    public Guid MatchId => Id;
    public IEnumerable<GetMatchResponse> Games { get; set; }
}
