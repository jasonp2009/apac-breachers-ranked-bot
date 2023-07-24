using ApacBreachersRanked.Domain.Common;

namespace ApacBreachersRanked.Domain.Match.Events
{
    public class AllPlayersConfirmedEvent : IDomainEvent
    {
        public Guid MatchId { get; set; }
    }
}
