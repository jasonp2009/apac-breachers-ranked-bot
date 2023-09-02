using ApacBreachersRanked.Domain.Common;
using ApacBreachersRanked.Domain.Match.Enums;

namespace ApacBreachersRanked.Domain.Match.Events
{
    public class MatchCancelledEvent : IDomainEvent
    {
        public Guid MatchId { get; set; }
        public MatchStatus PreviousStatus { get; set; }
    }
}
