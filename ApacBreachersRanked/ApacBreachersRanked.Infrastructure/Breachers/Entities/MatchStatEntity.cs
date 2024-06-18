using ApacBreachersRanked.Domain.Common;
using ApacBreachersRanked.Infrastructure.Breachers.Models;

namespace ApacBreachersRanked.Infrastructure.Breachers.Entities;

public class MatchStatEntity : BaseEntity
{
    public Guid MatchId { get; set; }
    public IEnumerable<GetMatchResponse> Games { get; set; }
}
