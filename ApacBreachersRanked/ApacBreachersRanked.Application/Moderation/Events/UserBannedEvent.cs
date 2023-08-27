using ApacBreachersRanked.Domain.Common;

namespace ApacBreachersRanked.Application.Moderation.Events
{
    public class UserBannedEvent : IDomainEvent
    {
        public Guid UserBanId { get; set; }
    }
}
