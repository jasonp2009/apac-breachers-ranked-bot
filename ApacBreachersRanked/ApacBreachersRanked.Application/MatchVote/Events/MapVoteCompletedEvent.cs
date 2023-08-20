using ApacBreachersRanked.Domain.Common;

namespace ApacBreachersRanked.Application.MatchVote.Events
{
    public class MapVoteCompletedEvent : IDomainEvent
    {
        public Guid MatchId { get; set; }
    }
}
