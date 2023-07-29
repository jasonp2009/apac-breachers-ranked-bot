using ApacBreachersRanked.Domain.Match.Enums;

namespace ApacBreachersRanked.Domain.Match.Entities
{
    public class MatchScore
    {
        public IList<MapScore> Maps { get; private set; } = new List<MapScore>();
        public Score RoundScore => new Score(Maps.Sum(map => map.Home),
                                             Maps.Sum(map => map.Away));
        public Score MapScore => new Score(Maps.Count(map => map.Outcome == ScoreOutcome.Home),
                                           Maps.Count(map => map.Outcome == ScoreOutcome.Away));
        public ScoreOutcome Outcome
            => MapScore.Home > MapScore.Away ? ScoreOutcome.Home
             : MapScore.Away > MapScore.Home ? ScoreOutcome.Away
                                             : ScoreOutcome.Draw;
    }
}
