using ApacBreachersRanked.Application.Moderation.Events;
using ApacBreachersRanked.Domain.Common;
using ApacBreachersRanked.Domain.User.Interfaces;

namespace ApacBreachersRanked.Application.Moderation.Models
{
    public class UserBan : BaseEntity, IUser
    {
        public IUserId UserId { get; private set; }
        public string? Name { get; private set; }
        public DateTime BannedAtUtc { get; private set; }
        public TimeSpan Duration { get; private set; }
        public DateTime ExpiryUtc { get; private init; }
        public string Reason { get; private set; }
        public bool UnBanOverride { get; private set; } = false;
        public string? UnBanReason { get; private set; }

        private UserBan() { }
        public UserBan(IUser user, TimeSpan duration, string reason)
        {
            if (string.IsNullOrWhiteSpace(reason)) throw new InvalidOperationException("A ban reason must be provided");
            UserId = user.UserId;
            Name = user.Name;
            BannedAtUtc = DateTime.UtcNow;
            Duration = duration;
            ExpiryUtc = BannedAtUtc.Add(duration);
            Reason = reason;
            QueueDomainEvent(new UserBannedEvent { UserBanId = Id });
            QueueDomainEvent(new UserBanExpiredEvent { UserBanId = Id, ScheduledForUtc = ExpiryUtc });
        }

        public void UnBan(string reason)
        {
            if (string.IsNullOrWhiteSpace(reason)) throw new InvalidOperationException("A unban reason must be provided");
            if (!UnBanOverride)
            {
                UnBanOverride = true;
                UnBanReason = reason;
                QueueDomainEvent(new UserUnBannedEvent { UserBanId = Id });
            }
        }
    }
}
