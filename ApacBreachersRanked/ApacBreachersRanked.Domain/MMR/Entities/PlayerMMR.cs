using ApacBreachersRanked.Domain.Common;
using ApacBreachersRanked.Domain.MMR.Events;
using ApacBreachersRanked.Domain.User.Interfaces;

namespace ApacBreachersRanked.Domain.MMR.Entities
{
    public class PlayerMMR : BaseEntity
    {
        public IUserId UserId { get; private set; } = null!;
        public decimal MMR { get; private set; } = 1000;
        public IList<MMRAdjustment> Adjustments { get; private set; } = new List<MMRAdjustment>();

        public PlayerMMR(IUserId userId)
        {
            UserId = userId;
        }

        public void ApplyAdjustment(MMRAdjustment adjustment)
        {
            MMR += adjustment.Adjustment;
            Adjustments.Add(adjustment);
            QueueDomainEvent(new MMRAdjustedEvent { UserId = UserId });
        }
    }
}
