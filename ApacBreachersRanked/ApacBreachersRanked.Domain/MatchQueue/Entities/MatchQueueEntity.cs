using ApacBreachersRanked.Domain.Common;
using ApacBreachersRanked.Domain.Helpers;
using ApacBreachersRanked.Domain.Match.Constants;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.Match.Enums;
using ApacBreachersRanked.Domain.MatchQueue.Events;
using ApacBreachersRanked.Domain.User.Interfaces;

namespace ApacBreachersRanked.Domain.MatchQueue.Entities
{
    public class MatchQueueEntity : BaseEntity
    {
        public MatchFormat MatchFormat { get; protected set; } = MatchConstants.DefaultMatchFormat;
        public bool IsOpen { get; protected set; } = true;
        public IList<MatchQueueUser> Users { get; private set; } = new List<MatchQueueUser>();
        public MatchEntity? Match { get; private set; }

        private MatchQueueEntity() {}

        public MatchQueueEntity(MatchFormat matchFormat)
        {
            MatchFormat = matchFormat;
        }
        public static MatchQueueEntity CreateNewQueueFromUsers(IEnumerable<MatchQueueUser> users, MatchFormat matchFormat = MatchConstants.DefaultMatchFormat)
        {
            MatchQueueEntity newQueue = new()
            {
                MatchFormat = matchFormat
            };
            foreach(MatchQueueUser user in users)
            {
                newQueue.Users.Add(new MatchQueueUser(user, user.ExpiryUtc, user.JoinedAtUtc));
            }
            newQueue.QueueDomainEvent(new MatchQueueUpdatedEvent { MatchQueueId = newQueue.Id });
            return newQueue;
        }
        public void AddUserToQueue(IUser user, DateTime expiryUtc, DateTime? joinedAtUtc = null)
        {
            MatchQueueUser? matchQueueUser = Users.FirstOrDefault(x => x.UserId.Equals(user.UserId));
            if (matchQueueUser != null)
            {
                matchQueueUser.UpdateExpiry(expiryUtc);
                if (joinedAtUtc != null && joinedAtUtc < matchQueueUser.JoinedAtUtc)
                {
                    matchQueueUser.UpdateJoinedAt(joinedAtUtc.Value);
                }
            } else
            {
                matchQueueUser = new(user, expiryUtc, joinedAtUtc);
                Users.Add(matchQueueUser);
                if (Users.Count >= MatchFormat.GetMatchConstant(c => c.MaxCapacity))
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

        public void ToggleVoteToForce(IUserId userId)
        {
            MatchQueueUser? existingUser = Users.FirstOrDefault(user => user.UserId.Equals(userId));
            if (existingUser != null)
            {
                existingUser.ToggleVoteToForce();
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
