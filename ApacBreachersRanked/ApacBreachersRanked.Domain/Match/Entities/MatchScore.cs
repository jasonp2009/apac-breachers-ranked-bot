using ApacBreachersRanked.Domain.Match.Enums;

namespace ApacBreachersRanked.Domain.Match.Entities
{
    public class MatchScore
    {
        public int Home { get; set; }
        public int Away { get; set; }

        public MatchScore(int home, int away)
        {
            Home = home;
            Away = away;
        }
    }
}
