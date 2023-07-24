using ApacBreachersRanked.Domain.Common;
using ApacBreachersRanked.Domain.Match.Entities;

namespace ApacBreachersRanked.Application.Match.Models
{
    public class MatchThreads : BaseEntity
    {
        public MatchEntity Match { get; set; }
        public ulong MatchThreadId { get; set; }
        public ulong MatchThreadWelcomeMessageId { get; set; }
        public ulong HomeThreadId { get; set; }
        public ulong AwayThreadId { get; set; }

        private MatchThreads() { }
        public MatchThreads(MatchEntity match)
        {
            Match = match;
        }
    }
}
