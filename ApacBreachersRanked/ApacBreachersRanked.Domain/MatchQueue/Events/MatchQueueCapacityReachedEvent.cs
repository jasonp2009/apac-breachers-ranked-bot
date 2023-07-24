using ApacBreachersRanked.Domain.Common;

namespace ApacBreachersRanked.Domain.MatchQueue.Events
{
    public class MatchQueueCapacityReachedEvent : IDomainEvent
    {
        public Guid MatchQueueId { get; set; }
    }
}
