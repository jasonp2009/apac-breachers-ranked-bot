using ApacBreachersRanked.Domain.User.Interfaces;

namespace ApacBreachersRanked.Application.MatchQueue.Models
{
    public class ScheduledMatchQueueUser : IUser
    {
        public IUserId UserId { get; private set; }
        public string? Name { get; private set; }
        public DateTime JoinAtUtc { get; private set; }
        public DateTime ExpiryUtc { get; private set; }
        
        public ScheduledMatchQueueUser(IUser user, DateTime joinAtUtc, DateTime expiryUtc)
        {
            UserId = user.UserId;
            Name = user.Name;
            JoinAtUtc = joinAtUtc;
            ExpiryUtc = expiryUtc;
        }
    }
}
