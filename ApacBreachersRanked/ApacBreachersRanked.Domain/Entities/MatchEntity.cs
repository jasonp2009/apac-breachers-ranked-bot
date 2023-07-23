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
        public MatchStatus Status { get; private set; } = MatchStatus.PendingConfirmation;
        public IList<MatchPlayer> HomePlayers { get; private set; } = null!;
        public IList<MatchPlayer> AwayPlayers { get; private set; } = null!;
        public IEnumerable<MatchPlayer> AllPlayers => HomePlayers.Concat(AwayPlayers);
        public MatchScore? Score { get; private set; } = null!;

        private MatchEntity() { }
        internal MatchEntity(MatchQueueEntity matchQueue, IList<IUser> home, IList<IUser> away)
        {
            matchQueue.SetMatch(this);
            HomePlayers = home.Select(x => new MatchPlayer(x)).ToList();
            AwayPlayers = away.Select(x => new MatchPlayer(x)).ToList();
            QueueDomainEvent(new MatchCreatedEvent() { MatchId = Id });
        }
    }

    public class MatchPlayer : BaseEntity, IUser
    {
        public IUserId UserId { get; set; }
        public string Name { get; set; }

        private MatchPlayer() { }
        public MatchPlayer(IUser user)
        {
            UserId = user.UserId;
            Name = user.Name;
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
}
