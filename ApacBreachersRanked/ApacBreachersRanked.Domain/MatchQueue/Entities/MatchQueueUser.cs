using ApacBreachersRanked.Domain.User.Interfaces;

namespace ApacBreachersRanked.Domain.MatchQueue.Entities
{
    public class MatchQueueUser : IUser
    {
        public IUserId UserId { get; private set; } = null!;
        public string? Name { get; private set; }
        public DateTime ExpiryUtc { get; private set; }
        public bool VoteToForce { get; private set; } = false;

        internal MatchQueueUser()
        {

        }
        internal MatchQueueUser(IUser user, DateTime expiryUtc)
        {
            UserId = user.UserId;
            Name = user.Name;
            ExpiryUtc = expiryUtc;
        }

        internal void UpdateExpiry(DateTime expiryUtc)
        {
            ExpiryUtc = expiryUtc;
        }

        internal void ToggleVoteToForce()
        {
            VoteToForce = !VoteToForce;
        }
    }
}
