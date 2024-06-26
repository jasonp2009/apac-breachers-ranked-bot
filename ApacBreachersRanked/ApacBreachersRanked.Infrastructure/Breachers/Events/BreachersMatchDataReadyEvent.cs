using ApacBreachersRanked.Domain.Common;

namespace ApacBreachersRanked.Infrastructure.Breachers.Events;

public class BreachersMatchDataReadyEvent : IDomainEvent
{
    public Guid MatchId { get; set; }
}
