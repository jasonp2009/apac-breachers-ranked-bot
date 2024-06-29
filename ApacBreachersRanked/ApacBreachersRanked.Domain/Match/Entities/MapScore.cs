using ApacBreachersRanked.Domain.Match.Enums;

namespace ApacBreachersRanked.Domain.Match.Entities
{
    public class MapScore : Score
    {
        public Map Map { get; private set; }
        public MapScore(Map map, int home, int away)
            : base(home, away)
        {
            Map = map;
        }
    }
}
