using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.User.Interfaces;

namespace ApacBreachersRanked.Domain.MMR.Entities
{
    public class MMRAdjustment
    {
        public IUserId UserId { get; private set; }
        public decimal Adjustment { get; private set; }
        public MatchEntity Match { get; private set; }

        private MMRAdjustment() { }

        public MMRAdjustment(IUserId userId, decimal adjustment, MatchEntity match)
        {
            UserId = userId;
            Adjustment = adjustment;
            Match = match;
        }
    }
}
