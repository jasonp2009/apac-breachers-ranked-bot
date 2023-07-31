using ApacBreachersRanked.Domain.Common;

namespace ApacBreachersRanked.Domain.MatchQueue.Events
{
    public class MatchQueueUpdatedEvent : IDomainEvent
    {
        public Guid? MatchQueueId { get; set; }
    }
}
