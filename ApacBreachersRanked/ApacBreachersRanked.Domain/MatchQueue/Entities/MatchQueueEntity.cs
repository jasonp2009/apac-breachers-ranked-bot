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
            MatchQueueUser? existingUser = Users.FirstOrDefault(x => x.UserId.Equals(user.UserId));
            if (existingUser != null)
            {
                existingUser.UpdateExpiry(expiryUtc);
                return;
            }
            MatchQueueUser matchQueueUser = new(user, expiryUtc);
            Users.Add(matchQueueUser);
            QueueDomainEvent(new MatchQueueUpdatedEvent { MatchQueueId = Id });
            if (Users.Count >= MatchConstants.MaxCapacity)
            {
                QueueDomainEvent(new MatchQueueCapacityReachedEvent { MatchQueueId = Id });
            }
        }

        public void RemoveUserFromQueue(IUser user)
        {
            MatchQueueUser? existingUser = Users.FirstOrDefault(x => x.UserId.Equals(user.UserId));
            if (existingUser != null)
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
