using ApacBreachersRanked.Domain.Common;

namespace ApacBreachersRanked.Domain.Match.Events
{
    public class MatchCancelledEvent : IDomainEvent
    {
        public Guid MatchId { get; set; }
    }
}
