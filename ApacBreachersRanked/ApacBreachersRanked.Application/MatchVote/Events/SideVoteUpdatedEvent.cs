using ApacBreachersRanked.Domain.Common;

namespace ApacBreachersRanked.Application.MatchVote.Events
{
    public class SideVoteUpdatedEvent : IDomainEvent
    {
        public Guid MatchId { get; set; }
    }
}
