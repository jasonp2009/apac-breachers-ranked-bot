using ApacBreachersRanked.Domain.Common;

namespace ApacBreachersRanked.Domain.MMR.Events
{
    public class MatchMMRCalculatedEvent : IDomainEvent
    {
        public Guid? MatchId { get; set; }
    }
}
