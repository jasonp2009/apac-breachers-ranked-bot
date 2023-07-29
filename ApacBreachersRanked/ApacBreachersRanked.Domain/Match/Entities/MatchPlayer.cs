using ApacBreachersRanked.Domain.Common;
using ApacBreachersRanked.Domain.Match.Enums;
using ApacBreachersRanked.Domain.Match.Events;
using ApacBreachersRanked.Domain.User.Interfaces;

namespace ApacBreachersRanked.Domain.Match.Entities
{
    public class MatchPlayer : BaseEntity, IUser
    {
        public MatchEntity Match { get; private set; }
        public IUserId UserId { get; private set; }
        public string Name { get; private set; }
        public MatchSide Side { get; private set; }
        public bool Confirmed { get; private set; } = false;
        public bool IsHost { get; private set; } = false;
        private MatchPlayer() { }
        public MatchPlayer(IUser user, MatchSide side)
        {
            UserId = user.UserId;
            Name = user.Name;
            Side = side;
        }

        public void Confirm()
        {
            Confirmed = true;
            QueueDomainEvent(new PlayerConfirmedEvent { MatchId = Match.Id, MatchPlayerId = Id });
            if (Match.AllPlayers.All(player => player.Confirmed))
            {
                QueueDomainEvent(new AllPlayersConfirmedEvent { MatchId = Match.Id });
            }
        }

        public void Reject()
        {
            Confirmed = false;
            Match.CancelMatch($"{Name} rejected the match");
        }

        public void SetPlayerAsHost()
        {
            IsHost = true;
        }
    }
}
