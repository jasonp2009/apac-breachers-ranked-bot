﻿using ApacBreachersRanked.Domain.Common;
using ApacBreachersRanked.Domain.Helpers;
using ApacBreachersRanked.Domain.Match.Enums;
using ApacBreachersRanked.Domain.Match.Events;
using ApacBreachersRanked.Domain.MatchQueue.Entities;
using ApacBreachersRanked.Domain.User.Interfaces;

namespace ApacBreachersRanked.Domain.Match.Entities
{

    public class MatchEntity : BaseEntity
    {
        public int MatchNumber { get; set; }
        public MatchStatus Status { get; private set; } = MatchStatus.PendingConfirmation;
        public IEnumerable<MatchPlayer> HomePlayers => AllPlayers.Where(player => player.Side == MatchSide.Home);
        public IEnumerable<MatchPlayer> AwayPlayers => AllPlayers.Where(player => player.Side == MatchSide.Away);
        public IList<MatchPlayer> AllPlayers { get; private set; } = new List<MatchPlayer>();
        public MatchPlayer? HostPlayer => AllPlayers.FirstOrDefault(player => player.IsHost);
        public MatchScore? Score { get; private set; } = null;
        public string? CancellationReason { get; private set; }
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

        public void ConfirmMatch()
        {
            if (AllPlayers.Any(player => !player.Confirmed))
            {
                throw new InvalidOperationException(
                    $"Unable to start the match as not all players have confirmed{Environment.NewLine}" +
                    $"Unconfirmed players:{Environment.NewLine}" +
                    $"{string.Join("\n", AllPlayers.Where(player => !player.Confirmed).Select(player => player.Name))}");
            }

            AllPlayers.SelectRandom().SetPlayerAsHost();

            Status = MatchStatus.Confirmed;
        }

        public void CancelMatch(string reason)
        {
            Status = MatchStatus.Cancelled;
            CancellationReason = reason;
            QueueDomainEvent(new MatchCancelledEvent { MatchId = Id });
        }

        public void SetScore(MatchScore score)
        {
            if (Score == null)
            {
                Status = MatchStatus.Completed;
                Score = score;
                QueueDomainEvent(new MatchScoreSetEvent { MatchId = Id });
            }
        }
    }
}
