using ApacBreachersRanked.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacBreachersRanked.Domain.Entities
{
    public class MatchCreatedEvent : IDomainEvent
    {
        public Guid MatchId { get; set; }
    }
    public class MatchEntity : BaseEntity
    {
        public int MatchNumber { get; set; }
        public MatchStatus Status { get; private set; } = MatchStatus.PendingConfirmation;
        public IEnumerable<MatchPlayer> HomePlayers => AllPlayers.Where(player => player.Side == MatchSide.Home);
        public IEnumerable<MatchPlayer> AwayPlayers => AllPlayers.Where(player => player.Side == MatchSide.Away);
        public IList<MatchPlayer> AllPlayers { get; private set; } = new List<MatchPlayer>();
        public MatchScore? Score { get; private set; } = null!;

        private MatchEntity() { }
        internal MatchEntity(MatchQueueEntity matchQueue, IList<IUser> home, IList<IUser> away)
        {
            matchQueue.SetMatch(this);
            foreach(IUser homePlayer in  home)
            {
                AllPlayers.Add(new MatchPlayer(homePlayer, MatchSide.Home));
            }
            foreach (IUser awayPlayer in away)
            {
                AllPlayers.Add(new MatchPlayer(awayPlayer, MatchSide.Away));
            }
            QueueDomainEvent(new MatchCreatedEvent() { MatchId = Id });
        }
    }

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

    public class MatchScore
    {
        public int Home { get; set; }
        public int Away { get; set; }
    }

    public enum MatchStatus
    {
        PendingConfirmation,
        Confirmed,
        Completed
    }

    public enum MatchSide
    {
        Home,
        Away
    }
}
