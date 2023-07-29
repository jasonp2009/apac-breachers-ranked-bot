using ApacBreachersRanked.Domain.Common;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.Match.Enums;
using ApacBreachersRanked.Domain.User.Interfaces;

namespace ApacBreachersRanked.Application.Match.Models
{
    public class PendingMatchScore : BaseEntity
    {
        public Guid MatchId { get; private set; }
        public MatchEntity Match { get; private set; }
        public MatchScore Score { get; private set; }
        public IList<PendingMatchScorePlayer> Players { get; private set; } = new List<PendingMatchScorePlayer>();
        public ulong MessageId { get; set; }
        private PendingMatchScore()
        {

        }

        public PendingMatchScore(MatchEntity match, MatchScore score)
        {
            MatchId = match.Id;
            Match = match;
            Score = score;
            foreach(var matchPlayer in match.AllPlayers)
            {
                Players.Add(new()
                {
                    UserId = matchPlayer.UserId,
                    Name = matchPlayer.Name,
                    Side = matchPlayer.Side
                });
            }
            QueueDomainEvent(new PendingScoreUpdatedEvent() { PendingMatchScoreId = Id });
        }

        public void SetPlayerConfirmationStatus(IUserId userId, bool confirmationStatus)
        {
            PendingMatchScorePlayer? player = Players.SingleOrDefault(player => player.UserId.Equals(userId));
            if (player == null) return;
            player.Confirmed = confirmationStatus;
            QueueDomainEvent(new PendingScoreUpdatedEvent { PendingMatchScoreId = Id });
            if (Players.Count(player => player.Confirmed) >= Math.Round((decimal)(Players.Count + 1)/2))
            {
                QueueDomainEvent(new PendingScoreConfirmedEvent { PendingMatchScoreId = Id });
            }
        }
    }

    public class PendingMatchScorePlayer : IUser
    {
        public IUserId UserId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public MatchSide Side { get; set; }
        public bool Confirmed { get; set; } = false;
    }

    public class PendingScoreUpdatedEvent : IDomainEvent
    {
        public Guid PendingMatchScoreId { get; set; }
    }

    public class PendingScoreConfirmedEvent : IDomainEvent
    {
        public Guid PendingMatchScoreId { get; set; }
    }
}
