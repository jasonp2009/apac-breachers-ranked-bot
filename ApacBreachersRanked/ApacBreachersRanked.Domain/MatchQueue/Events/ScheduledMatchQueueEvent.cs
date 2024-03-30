using ApacBreachersRanked.Domain.Common;
using ApacBreachersRanked.Domain.MatchQueue.Entities;

namespace ApacBreachersRanked.Domain.MatchQueue.Events
{
    public class ScheduledMatchQueueEvent : IScheduledEvent
    {
        public Guid ScheduledMatchQueueId { get; init; }
        public DateTime ScheduledForUtc { get; init; }

        public static ScheduledMatchQueueEvent FromScheduledMatchQueue(ScheduledMatchQueueEntity scheduledMatchQueue)
        {
            var scheduledMatchQueueEvent = new ScheduledMatchQueueEvent
            {
                ScheduledMatchQueueId = scheduledMatchQueue.Id,
                ScheduledForUtc = scheduledMatchQueue.ScheduledForUtc
            };
            return scheduledMatchQueueEvent;
        }
    }
}
