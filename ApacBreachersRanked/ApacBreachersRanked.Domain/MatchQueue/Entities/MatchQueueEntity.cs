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

        public static MatchQueueEntity CreateNewQueueFromUsers(IEnumerable<MatchQueueUser> users)
        {
            MatchQueueEntity newQueue = new();
            foreach(MatchQueueUser user in users)
            {
                newQueue.Users.Add(new MatchQueueUser(user, user.ExpiryUtc));
            }
            newQueue.QueueDomainEvent(new MatchQueueUpdatedEvent { MatchQueueId = newQueue.Id });
            return newQueue;
        }
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

        public void CloseQueueAndSetMatch(MatchEntity match)
        {
            Match = match;
            IsOpen = false;
            QueueDomainEvent(new MatchQueueClosedEvent { MatchQueueId = Id });
        }
    }
}
