using ApacBreachersRanked.Domain.Common;
using ApacBreachersRanked.Domain.MMR.Enums;
using ApacBreachersRanked.Domain.MMR.Events;
using ApacBreachersRanked.Domain.MMR.Helpers;
using ApacBreachersRanked.Domain.User.Interfaces;

namespace ApacBreachersRanked.Domain.MMR.Entities
{
    public class PlayerMMR : BaseEntity, IUser
    {
        public IUserId UserId { get; private set; } = null!;
        public string? Name { get; set; }
        public decimal MMR { get; private set; } = 1000;
        public Rank? Rank { get; private set; }
        public IList<MMRAdjustment> Adjustments { get; private set; } = new List<MMRAdjustment>();

        private PlayerMMR() { }

        public PlayerMMR(IUser user, decimal? mmr = null, Rank? rank = null)
        {
            UserId = user.UserId;
            Name = user.Name;
            MMR = mmr ?? 1000;
            Rank = rank;
        }

        public void ApplyAdjustment(MMRAdjustment adjustment)
        {
            decimal newMMR = MMR + adjustment.Adjustment;

            if (Rank == null)
            {
                Rank = RankHelpers.GetRankForMMR(newMMR);
            } else
            {
                Rank rankByMMR = RankHelpers.GetRankForMMR(MMR);
                Rank newRankByMMR = RankHelpers.GetRankForMMR(newMMR);
                if (Rank != rankByMMR && rankByMMR == newRankByMMR)
                {
                    bool isRankUp = (int)Rank < (int)newRankByMMR;
                    bool shouldUpdateRank = (isRankUp && adjustment.Adjustment > 0) || (!isRankUp && adjustment.Adjustment < 0);

                    if (shouldUpdateRank)
                    {
                        Rank = newRankByMMR;
                        QueueDomainEvent(new PlayerRankUpdatedEvent { UserId = UserId });
                    }
                }
            }

            MMR = newMMR;
            Adjustments.Add(adjustment);
            QueueDomainEvent(new PlayerMMRAdjustedEvent { UserId = UserId });
        }
    }
}
