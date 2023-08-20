using ApacBreachersRanked.Domain.Common;

namespace ApacBreachersRanked.Application.MatchVote.Events
{
    public class MapVoteUpdatedEvent : IDomainEvent
    {
        public Guid MatchId { get; set; }
    }
}
