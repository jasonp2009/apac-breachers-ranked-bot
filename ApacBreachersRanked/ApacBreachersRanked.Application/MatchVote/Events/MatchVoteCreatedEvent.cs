using ApacBreachersRanked.Domain.Common;

namespace ApacBreachersRanked.Application.MatchVote.Events
{
    public class MatchVoteCreatedEvent : IDomainEvent
    {
        public Guid MatchId { get; set; }
    }
}
