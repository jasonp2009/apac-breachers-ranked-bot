using ApacBreachersRanked.Domain.Match.Enums;
using ApacBreachersRanked.Domain.User.Interfaces;

namespace ApacBreachersRanked.Application.MatchVote.Models
{
    public class MatchVoteMap : IUser
    {
        public IUserId UserId { get; private set; }
        public string? Name { get; private set; }
        public Map? Vote { get; private set; } = null;


        private MatchVoteMap()
        {

        }

        public MatchVoteMap(IUser user)
        {
            UserId = user.UserId;
            Name = user.Name;
        }

        public void SetVote(Map vote)
        {
            if (Vote == null) Vote = vote;
        }
    }
}
