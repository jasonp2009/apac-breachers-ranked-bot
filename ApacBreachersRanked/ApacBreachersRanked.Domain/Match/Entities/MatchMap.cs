using ApacBreachersRanked.Domain.Common;
using ApacBreachersRanked.Domain.Match.Enums;

namespace ApacBreachersRanked.Domain.Match.Entities
{
    public class MatchMap
    {
        public Map Map { get; private set; }
        public MatchScore Score { get; private set; }

        private MatchMap() { }
        public MatchMap(Map map, MatchScore score)
        {
            Map = map;
            Score = score;
        }
    }
}
