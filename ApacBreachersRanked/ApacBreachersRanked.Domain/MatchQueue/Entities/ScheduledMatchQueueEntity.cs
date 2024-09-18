using ApacBreachersRanked.Domain.Common;
using ApacBreachersRanked.Domain.Match.Constants;
using ApacBreachersRanked.Domain.Match.Enums;
using ApacBreachersRanked.Domain.MatchQueue.Events;
using ApacBreachersRanked.Domain.User.Interfaces;

namespace ApacBreachersRanked.Domain.MatchQueue.Entities
{
    public class ScheduledMatchQueueEntity : BaseEntity
    {
        public MatchFormat MatchFormat { get; protected set; } = MatchConstants.DefaultMatchFormat;
        public bool IsOpen { get; protected set; } = true;
        public DateTime ScheduledForUtc { get; private set; }
        public IList<MatchQueueUser> Users { get; private set; } = new List<MatchQueueUser>();

        private ScheduledMatchQueueEntity()
        {
        
        }

        public ScheduledMatchQueueEntity(DateTime scheduledForUtc, MatchFormat matchFormat)
        {
            MatchFormat = matchFormat;
            ScheduledForUtc = scheduledForUtc;
            QueueDomainEvent(ScheduledMatchQueueEvent.FromScheduledMatchQueue(this));
        }

        public void AddUserToQueue(IUser user)
        {
            MatchQueueUser? matchQueueUser = Users.FirstOrDefault(x => x.UserId.Equals(user.UserId));
            if (matchQueueUser != null)
            {
                matchQueueUser.UpdateExpiry(ScheduledForUtc + TimeSpan.FromMinutes(30));
            } else
            {
                matchQueueUser = new(user, ScheduledForUtc + TimeSpan.FromMinutes(30));
                Users.Add(matchQueueUser);
            }
            QueueDomainEvent(new ScheduledMatchQueueUpdatedEvent { MatchQueueId = Id });
        }

        public void RemoveUserFromQueue(IUserId userId)
        {
            MatchQueueUser? existingUser = Users.FirstOrDefault(x => x.UserId.Equals(userId));
            if (existingUser != null)
            {
                Users.Remove(existingUser);
                QueueDomainEvent(new ScheduledMatchQueueUpdatedEvent { MatchQueueId = Id });
            }
        }

        public void CloseQueue()
        {
            IsOpen = false;
            QueueDomainEvent(new ScheduledMatchQueueClosedEvent { MatchQueueId = Id });
        }
    }
}
