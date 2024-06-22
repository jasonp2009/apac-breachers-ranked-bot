using ApacBreachersRanked.Domain.Common;

namespace ApacBreachersRanked.Infrastructure.Breachers.Events;

public class PollForMatchDataEvent : IScheduledEvent
{
    public Guid MatchId { get; set; }
    public DateTime ScheduledForUtc { get; set; }
}
