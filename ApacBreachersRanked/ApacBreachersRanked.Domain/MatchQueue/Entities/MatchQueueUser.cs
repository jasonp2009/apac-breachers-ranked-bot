using ApacBreachersRanked.Domain.User.Interfaces;

namespace ApacBreachersRanked.Domain.MatchQueue.Entities
{
    public class MatchQueueUser : IUser
    {
        public IUserId UserId { get; private set; } = null!;
        public string? Name { get; private set; }
        public DateTime ExpiryUtc { get; private set; }
        public DateTime JoinedAtUtc { get; private set; }
        public bool VoteToForce { get; private set; } = false;

        internal MatchQueueUser()
        {

        }
        internal MatchQueueUser(IUser user, DateTime expiryUtc, DateTime? joinedAtUtc = null)
        {
            UserId = user.UserId;
            Name = user.Name;
            ExpiryUtc = expiryUtc;
            JoinedAtUtc = joinedAtUtc ?? DateTime.UtcNow;
        }

        internal void UpdateExpiry(DateTime expiryUtc)
        {
            ExpiryUtc = expiryUtc;
        }

        internal void UpdateJoinedAt(DateTime joinedAt)
        {
            JoinedAtUtc = joinedAt;
        }

        internal void ToggleVoteToForce()
        {
            VoteToForce = !VoteToForce;
        }
    }
}
