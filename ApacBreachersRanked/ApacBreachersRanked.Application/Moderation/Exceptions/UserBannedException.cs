using ApacBreachersRanked.Application.Moderation.Models;
using ApacBreachersRanked.Domain.User.Interfaces;

namespace ApacBreachersRanked.Application.Moderation.Exceptions
{
    public class UserBannedException : Exception, IUser
    {
        public IUserId UserId { get; set; }
        public string? Name { get; set; }
        public DateTime ExpiryUtc { get; set; }
        public string Reason { get; set; }

        public UserBannedException(UserBan ban)
            : base($"{ban.Name}({ban.UserId}) is banned until {ban.ExpiryUtc}.{Environment.NewLine}" +
                   $"Reason: {ban.Reason}")
        {
            UserId = ban.UserId;
            Name = ban.Name;
            ExpiryUtc = ban.ExpiryUtc;
            Reason = ban.Reason;
        }
    }
}
