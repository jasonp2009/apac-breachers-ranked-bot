using ApacBreachersRanked.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ApacBreachersRanked.Domain.Entities
{
    public class MatchQueueEntity : BaseEntity
    {
        public bool IsOpen { get; private set; } = true;
        public IList<MatchQueueUser> Users { get; private set; } = new List<MatchQueueUser>();

        public void AddUserToQueue(IUser user, DateTime expiryUtc)
        {
            MatchQueueUser? existingUser = Users.FirstOrDefault(x => x.UserId.Equals(user.UserId));
            if (existingUser !=  null)
            {
                existingUser.UpdateExpiry(expiryUtc);
                return;
            }
            MatchQueueUser matchQueueUser = new(user, expiryUtc);
            Users.Add(matchQueueUser);
            if (Users.Count >= 10)
            {
                QueueDomainEvent(new MatchQueueCapacityReachedEvent { MatchQueueId = Id });
            }
            QueueDomainEvent(new MatchQueueUpdatedEvent { MatchQueueId = Id });
        }
    }

    public class MatchQueueCapacityReachedEvent : IDomainEvent
    {
        public Guid MatchQueueId { get; set; }
    }

    public class MatchQueueUpdatedEvent : IDomainEvent
    {
        public Guid MatchQueueId { get; set; }
    }


    public class MatchQueueUser : BaseEntity
    {
        public IUserId UserId { get; private set; } = null!;
        public DateTime ExpiryUtc { get; private set; }

        internal MatchQueueUser()
        {

        }
        internal MatchQueueUser(IUser user, DateTime expiryUtc)
        {
            UserId = user.UserId;
            ExpiryUtc = expiryUtc;
        }

        public void UpdateExpiry(DateTime expiryUtc)
        {
            ExpiryUtc = expiryUtc;
        }
    }
}
