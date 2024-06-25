using ApacBreachersRanked.Domain.Common;

namespace ApacBreachersRanked.Domain.MatchData.Events;

public class MatchDataReadyEvent : IDomainEvent
{
    public Guid MatchId { get; set; }
}
