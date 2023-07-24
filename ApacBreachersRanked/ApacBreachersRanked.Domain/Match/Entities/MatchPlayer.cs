using ApacBreachersRanked.Domain.Common;
using ApacBreachersRanked.Domain.Match.Enums;
using ApacBreachersRanked.Domain.User.Interfaces;

namespace ApacBreachersRanked.Domain.Match.Entities
{
    public class MatchPlayer : BaseEntity, IUser
    {
        public IUserId UserId { get; set; }
        public string Name { get; set; }
        public MatchSide Side { get; set; }

        private MatchPlayer() { }
        public MatchPlayer(IUser user, MatchSide side)
        {
            UserId = user.UserId;
            Name = user.Name;
            Side = side;
        }
    }
}
