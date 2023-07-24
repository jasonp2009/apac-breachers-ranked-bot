using ApacBreachersRanked.Domain.Common;
using ApacBreachersRanked.Domain.User.Interfaces;

namespace ApacBreachersRanked.Domain.MatchQueue.Entities
{
    public class MatchQueueUser : BaseEntity, IUser
    {
        public IUserId UserId { get; private set; } = null!;
        public string Name { get; private set; } = null!;
        public DateTime ExpiryUtc { get; private set; }


        internal MatchQueueUser()
        {

        }
        internal MatchQueueUser(IUser user, DateTime expiryUtc)
        {
            UserId = user.UserId;
            Name = user.Name;
            ExpiryUtc = expiryUtc;
        }

        public void UpdateExpiry(DateTime expiryUtc)
        {
            ExpiryUtc = expiryUtc;
        }
    }
}
