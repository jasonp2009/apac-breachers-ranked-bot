using ApacBreachersRanked.Domain.Common;

namespace ApacBreachersRanked.Domain.MatchQueue.Events
{
    public class ScheduledMatchQueueUpdatedEvent : IDomainEvent
    {
        public Guid? MatchQueueId { get; set; }
    }
}
