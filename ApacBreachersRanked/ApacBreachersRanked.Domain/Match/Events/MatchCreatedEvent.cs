using ApacBreachersRanked.Domain.Common;

namespace ApacBreachersRanked.Domain.Match.Events
{
    public class MatchCreatedEvent : IDomainEvent
    {
        public Guid MatchId { get; set; }
    }
}
