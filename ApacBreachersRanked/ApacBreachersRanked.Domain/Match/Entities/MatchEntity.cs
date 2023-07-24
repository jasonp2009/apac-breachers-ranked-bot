using ApacBreachersRanked.Domain.Common;
using ApacBreachersRanked.Domain.Match.Enums;
using ApacBreachersRanked.Domain.Match.Events;
using ApacBreachersRanked.Domain.MatchQueue.Entities;
using ApacBreachersRanked.Domain.User.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacBreachersRanked.Domain.Match.Entities
{

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
            foreach (IUser homePlayer in home)
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
}
