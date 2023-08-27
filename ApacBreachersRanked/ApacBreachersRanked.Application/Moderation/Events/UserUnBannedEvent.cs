using ApacBreachersRanked.Domain.Common;

namespace ApacBreachersRanked.Application.Moderation.Events
{
    public class UserUnBannedEvent : IDomainEvent
    {
        public Guid UserBanId { get; set; }
    }
}
