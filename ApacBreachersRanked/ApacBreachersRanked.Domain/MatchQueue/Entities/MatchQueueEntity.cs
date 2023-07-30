using ApacBreachersRanked.Domain.Common;
using ApacBreachersRanked.Domain.Match.Constants;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.MatchQueue.Events;
using ApacBreachersRanked.Domain.User.Interfaces;

namespace ApacBreachersRanked.Domain.MatchQueue.Entities
{
    public class MatchQueueEntity : BaseEntity
    {
        public bool IsOpen { get; private set; } = true;
        public IList<MatchQueueUser> Users { get; private set; } = new List<MatchQueueUser>();
        public MatchEntity? Match { get; private set; }


        public void AddUserToQueue(IUser user, DateTime expiryUtc)
        {
            MatchQueueUser? matchQueueUser = Users.FirstOrDefault(x => x.UserId.Equals(user.UserId));
            if (matchQueueUser != null)
            {
                matchQueueUser.UpdateExpiry(expiryUtc);
            } else
            {
                matchQueueUser = new(user, expiryUtc);
                Users.Add(matchQueueUser);
                if (Users.Count >= MatchConstants.MaxCapacity)
                {
                    QueueDomainEvent(new MatchQueueCapacityReachedEvent { MatchQueueId = Id });
                }
            }
            QueueDomainEvent(new MatchQueueUpdatedEvent { MatchQueueId = Id });
            QueueDomainEvent(new MatchQueueUserExpiredEvent { ScheduledForUtc = expiryUtc, MatchQueueUserId = matchQueueUser.UserId });
        }

        public void RemoveUserFromQueue(IUserId userId)
        {
            MatchQueueUser? existingUser = Users.FirstOrDefault(x => x.UserId.Equals(userId));
            if (existingUser != null)
            {
                Users.Remove(existingUser);
                QueueDomainEvent(new MatchQueueUpdatedEvent { MatchQueueId = Id });
                return;
            }
        }

        public void ExpireUser(IUserId userId)
        {
            MatchQueueUser? existingUser = Users.FirstOrDefault(x => x.UserId.Equals(userId));
            if (existingUser != null && existingUser.ExpiryUtc <= DateTime.UtcNow)
            {
                Users.Remove(existingUser);
                QueueDomainEvent(new MatchQueueUpdatedEvent { MatchQueueId = Id });
                return;
            }
        }

        public void CloseQueue()
        {
            IsOpen = false;
        }

        public void SetMatch(MatchEntity match)
        {
            Match = match;
        }
    }
}
