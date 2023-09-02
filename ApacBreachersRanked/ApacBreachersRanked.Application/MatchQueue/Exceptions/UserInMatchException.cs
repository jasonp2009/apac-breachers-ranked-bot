using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.User.Interfaces;

namespace ApacBreachersRanked.Application.MatchQueue.Exceptions
{
    public class UserInMatchException : Exception, IUser
    {
        public IUserId UserId { get; private set; }
        public string? Name { get; private set; }
        public Guid MatchId { get; private set; }
        public int MatchNumber { get; private set; }

        public UserInMatchException(IUser user, MatchEntity match)
        {
            UserId = user.UserId;
            Name = user.Name;
            MatchId = match.Id;
            MatchNumber = match.MatchNumber;
        }
    }
}
