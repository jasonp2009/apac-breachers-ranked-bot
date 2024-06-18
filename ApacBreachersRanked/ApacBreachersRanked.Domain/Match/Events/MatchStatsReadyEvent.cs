using ApacBreachersRanked.Domain.Common;

namespace ApacBreachersRanked.Domain.Match.Events;

public class MatchStatsReadyEvent : IDomainEvent
{
    public Guid MatchId { get; set; }
}
