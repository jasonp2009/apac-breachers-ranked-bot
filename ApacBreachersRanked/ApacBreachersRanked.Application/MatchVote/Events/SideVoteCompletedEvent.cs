using ApacBreachersRanked.Domain.Common;

namespace ApacBreachersRanked.Application.MatchVote.Events
{
    public class SideVoteCompletedEvent : IDomainEvent
    {
        public Guid MatchId { get; set; }
    }
}
