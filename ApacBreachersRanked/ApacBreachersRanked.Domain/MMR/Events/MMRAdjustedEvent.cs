using ApacBreachersRanked.Domain.Common;
using ApacBreachersRanked.Domain.User.Interfaces;

namespace ApacBreachersRanked.Domain.MMR.Events
{
    public class MMRAdjustedEvent : IDomainEvent
    {
        public IUserId UserId { get; set; }
    }
}
