using ApacBreachersRanked.Domain.Common;
using ApacBreachersRanked.Domain.Match.Enums;
using ApacBreachersRanked.Domain.MMR.Constants;
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
        public MatchFormat MatchFormat { get; set; }
        public decimal MMR { get; set; } = 1000;
        public Rank? Rank { get; private set; }
        public IList<MMRAdjustment> Adjustments { get; private set; } = new List<MMRAdjustment>();

        private PlayerMMR() { }

        public PlayerMMR(IUser user, MatchFormat matchFormat, decimal? mmr = null, Rank? rank = null)
        {
            UserId = user.UserId;
            Name = user.Name;
            MatchFormat = matchFormat; 
            MMR = mmr ?? 1000;
            Rank = rank;
        }

        public void ApplyAdjustment(MMRAdjustment adjustment)
        {
            Adjustments.Add(adjustment);
            var oldMmr = MMR;
            MMR += adjustment.Adjustment;

            if (Adjustments.Count < MmrConstants.UnrankedMatches)
            {
                Rank = null;
            }
            else if (Rank == null)
            {
                Rank = RankHelpers.GetRankForMMR(MMR);
            }
            else
            {
                try
                {
                    Rank rankByMMR = RankHelpers.GetRankForMMR(oldMmr);
                    Rank newRankByMMR = RankHelpers.GetRankForMMR(MMR);
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
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            QueueDomainEvent(new PlayerMMRAdjustedEvent { UserId = UserId });
        }
    }
}
