﻿using ApacBreachersRanked.Domain.Common;
using ApacBreachersRanked.Domain.Helpers;
using ApacBreachersRanked.Domain.Match.Constants;
using ApacBreachersRanked.Domain.Match.Enums;
using ApacBreachersRanked.Domain.Match.Events;
using ApacBreachersRanked.Domain.MatchQueue.Entities;
using ApacBreachersRanked.Domain.MatchQueue.Events;
using ApacBreachersRanked.Domain.User.Interfaces;

namespace ApacBreachersRanked.Domain.Match.Entities
{

    public class MatchEntity : BaseEntity
    {
        public int MatchNumber { get; set; }
        public MatchStatus Status { get; private set; } = MatchStatus.PendingConfirmation;
        public DateTime AutoCancelDateUtc { get; init; } = DateTime.UtcNow + TimeSpan.FromMinutes(MatchConstants.AutoCancelMins);
        public IEnumerable<MatchPlayer> HomePlayers => AllPlayers.Where(player => player.Side == MatchSide.Home);
        public IEnumerable<MatchPlayer> AwayPlayers => AllPlayers.Where(player => player.Side == MatchSide.Away);
        public IList<MatchPlayer> AllPlayers { get; private set; } = new List<MatchPlayer>();
        public MatchPlayer? HostPlayer => AllPlayers.FirstOrDefault(player => player.IsHost);
        public MatchScore? Score { get; private set; } = null;
        public string? CancellationReason { get; private set; }
        private MatchEntity() { }
        internal MatchEntity(MatchQueueEntity matchQueue, IList<IUser> home, IList<IUser> away)
        {
            matchQueue.CloseQueueAndSetMatch(this);
            foreach (IUser homePlayer in home)
            {
                AllPlayers.Add(new MatchPlayer(homePlayer, MatchSide.Home));
            }
            foreach (IUser awayPlayer in away)
            {
                AllPlayers.Add(new MatchPlayer(awayPlayer, MatchSide.Away));
            }
            QueueDomainEvent(new MatchCreatedEvent { MatchId = Id });
            QueueDomainEvent(new AutoCancelMatchEvent { ScheduledForUtc = AutoCancelDateUtc, MatchId = Id });
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
            
            QueueDomainEvent(new MatchConfirmedEvent { MatchId = Id });
        }

        public void CancelMatch(string reason)
        {
            Status = MatchStatus.Cancelled;
            CancellationReason = reason;
            QueueDomainEvent(new MatchCancelledEvent { MatchId = Id });
            QueueDomainEvent(new MatchQueueUpdatedEvent());
        }

        public void AutoCancel()
        {
            if (Status == MatchStatus.PendingConfirmation &&
                AutoCancelDateUtc <= DateTime.UtcNow)
            {
                CancelMatch($"The match is being automatically cancelled due to the following players not confirming in time: {Environment.NewLine}" +
                            string.Join(Environment.NewLine, AllPlayers.Where(player => !player.Confirmed).Select(player => player.Name)));
            }
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
