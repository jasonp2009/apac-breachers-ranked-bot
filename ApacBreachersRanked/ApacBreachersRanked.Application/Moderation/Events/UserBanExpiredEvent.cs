using ApacBreachersRanked.Domain.Common;

namespace ApacBreachersRanked.Application.Moderation.Events
{
    public class UserBanExpiredEvent : IScheduledEvent
    {
        public DateTime ScheduledForUtc { get; set; }
        public Guid UserBanId { get; set; }
    }
}
