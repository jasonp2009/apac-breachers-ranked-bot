using ApacBreachersRanked.Domain.Common;

namespace ApacBreachersRanked.Domain.MatchData.Events;

public class GameDataReadyEvent : IDomainEvent
{
    public Guid MatchId { get; set; }
}
