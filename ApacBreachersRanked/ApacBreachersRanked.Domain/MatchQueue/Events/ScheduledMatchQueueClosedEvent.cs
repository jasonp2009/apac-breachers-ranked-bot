using ApacBreachersRanked.Domain.Common;

namespace ApacBreachersRanked.Domain.MatchQueue.Events
{
    public class ScheduledMatchQueueClosedEvent : IDomainEvent
    {
        public Guid MatchQueueId { get; set; }
    }
}
