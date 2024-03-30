using ApacBreachersRanked.Domain.Common;
using ApacBreachersRanked.Domain.MatchQueue.Entities;

namespace ApacBreachersRanked.Application.MatchQueue.Models
{
    public class ScheduledMatchQueueMessage : BaseEntity
    {
        public ScheduledMatchQueueEntity MatchQueue { get; set; } = null!;
        public ulong DiscordMessageId { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
