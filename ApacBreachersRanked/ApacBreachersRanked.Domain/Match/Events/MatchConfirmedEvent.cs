using ApacBreachersRanked.Domain.Common;

namespace ApacBreachersRanked.Domain.Match.Events
{
    public class MatchConfirmedEvent : IDomainEvent
    {
        public Guid MatchId { get; set; }
    }
}
