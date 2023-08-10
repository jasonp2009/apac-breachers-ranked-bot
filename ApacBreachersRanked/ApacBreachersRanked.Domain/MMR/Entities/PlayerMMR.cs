using ApacBreachersRanked.Domain.Common;
using ApacBreachersRanked.Domain.MMR.Events;
using ApacBreachersRanked.Domain.User.Interfaces;

namespace ApacBreachersRanked.Domain.MMR.Entities
{
    public class PlayerMMR : BaseEntity, IUser
    {
        public IUserId UserId { get; private set; } = null!;
        public string? Name { get; set; }
        public decimal MMR { get; private set; } = 1000;
        public IList<MMRAdjustment> Adjustments { get; private set; } = new List<MMRAdjustment>();

        private PlayerMMR() { }

        public PlayerMMR(IUser user, decimal? mmr = null)
        {
            UserId = user.UserId;
            Name = user.Name;
            MMR = mmr ?? 1000;
        }

        public void ApplyAdjustment(MMRAdjustment adjustment)
        {
            MMR += adjustment.Adjustment;
            Adjustments.Add(adjustment);
            QueueDomainEvent(new PlayerMMRAdjustedEvent { UserId = UserId });
        }
    }
}
