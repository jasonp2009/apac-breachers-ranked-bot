using ApacBreachersRanked.Domain.Common;
using ApacBreachersRanked.Domain.Match.Enums;
using ApacBreachersRanked.Domain.Match.Events;
using ApacBreachersRanked.Domain.MMR.Enums;
using ApacBreachersRanked.Domain.User.Interfaces;

namespace ApacBreachersRanked.Domain.Match.Entities
{
    public class MatchPlayer : BaseEntity, IUser
    {
        public MatchEntity Match { get; private set; }
        public IUserId UserId { get; private set; }
        public string? Name { get; private set; }
        public decimal MMR { get; private set; }
        public Rank? Rank { get; private set; }
        public MatchSide Side { get; private set; }
        public bool Confirmed { get; private set; } = false;
        public bool IsHost { get; private set; } = false;
        private MatchPlayer() { }
        public MatchPlayer(IUser user, decimal mmr, Rank? rank, MatchSide side)
        {
            UserId = user.UserId;
            Name = user.Name;
            MMR = mmr;
            Rank = rank;
            Side = side;
        }

        public void Confirm()
        {
            Confirmed = true;
            QueueDomainEvent(new PlayerConfirmedEvent { MatchId = Match.Id, MatchPlayerId = Id });
            if (Match.AllPlayers.All(player => player.Confirmed))
            {
                Match.ConfirmMatch();
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

        public void SetMMR(decimal mmr)
        {
            MMR = mmr;
        }

        public void SetRank(Rank? rank)
        {
            Rank = rank;
        }
    }
}
