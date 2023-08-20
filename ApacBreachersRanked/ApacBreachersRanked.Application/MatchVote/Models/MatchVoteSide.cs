using ApacBreachersRanked.Application.MatchVote.Enums;
using ApacBreachersRanked.Domain.User.Interfaces;

namespace ApacBreachersRanked.Application.MatchVote.Models
{
    public class MatchVoteSide : IUser
    {
        public IUserId UserId { get; private set; }
        public string? Name { get; private set; }
        public GameSide? Vote { get; private set; } = null;

        private MatchVoteSide()
        {

        }

        public MatchVoteSide(IUser user)
        {
            UserId = user.UserId;
            Name = user.Name;
        }

        public void SetVote(GameSide vote)
        {
            if (Vote == null)
            {
                Vote = vote;
            }
        }
    }
}
