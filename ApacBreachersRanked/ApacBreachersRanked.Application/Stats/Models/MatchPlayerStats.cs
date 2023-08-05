using ApacBreachersRanked.Application.Stats.Enums;

namespace ApacBreachersRanked.Application.Stats.Models
{
    public class MatchPlayerStats
    {
        public int MatchNumber { get; internal set; }
        public MatchStatOutcome Outcome { get; internal set; }
        public int RoundsWon { get; internal set; }
        public int RoundsLost { get; internal set; }
        public decimal MMRAdjustment { get; internal set; }
    }
}
