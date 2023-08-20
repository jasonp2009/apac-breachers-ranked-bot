using ApacBreachersRanked.Domain.Common;

namespace ApacBreachersRanked.Application.MatchVote.Events
{
    public class SideVoteTimedOutEvent : IScheduledEvent
    {
        public DateTime ScheduledForUtc { get; set; }
        public Guid MatchId { get; set; }
    }
}
