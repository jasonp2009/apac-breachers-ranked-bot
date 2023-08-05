using ApacBreachersRanked.Domain.User.Interfaces;

namespace ApacBreachersRanked.Application.Stats.Models
{
    public class MatchesPlayerStats
    {
        public IUser User { get; internal set; }
        public decimal MMR { get; internal set; }
        public IList<MatchPlayerStats> Matches { get; internal set; } = new List<MatchPlayerStats>();
    }
}
