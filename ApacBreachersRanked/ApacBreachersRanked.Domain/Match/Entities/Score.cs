using ApacBreachersRanked.Domain.Match.Enums;

namespace ApacBreachersRanked.Domain.Match.Entities
{
    public class Score
    {
        public int Home { get; private set; }
        public int Away { get; private set; }
        public ScoreOutcome Outcome
            => Home > Away ? ScoreOutcome.Home
             : Away > Home ? ScoreOutcome.Away
                           : ScoreOutcome.Draw;

        public Score(int home, int away)
        {
            Home = home;
            Away = away;
        }

        public override string ToString()
            => $"{Home}-{Away}";
    }
}
