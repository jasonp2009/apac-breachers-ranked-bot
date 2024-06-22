using ApacBreachersRanked.Domain.Common;

namespace ApacBreachersRanked.Domain.Match.Events;

public class MatchDataReadyEvent : IDomainEvent
{
    public Guid MatchId { get; set; }
}
