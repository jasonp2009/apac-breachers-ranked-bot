using ApacBreachersRanked.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacBreachersRanked.Domain.Entities
{
    public class MatchQueueEntity : BaseEntity
    {
        public bool IsOpen { get; private set; }
        public IList<MatchQueueUser> Users { get; private set; } = new List<MatchQueueUser>();

        public void AddUserToQueue(User user, DateTime expiryUtc)
        {
            MatchQueueUser matchQueueUser = new(user, expiryUtc);
            Users.Add(matchQueueUser);
            if (Users.Count >= 10)
            {
                QueueDomainEvent(new QueueCapacityReachedEvent { MatchQueueId = Id });
            }
        }
    }

    public class MatchQueueUser
    {
        public Guid UserId { get; private set; }
        public DateTime ExpiryUtc { get; private set; }

        internal MatchQueueUser(User user, DateTime expiryUtc)
        {
            UserId = user.Id;
            ExpiryUtc = expiryUtc;
        }
    }
}
