using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.MMR.Entities;
using EloCalculator;

namespace ApacBreachersRanked.Domain.MMR.Services
{
    public interface IMMRAdjustmentService
    {
        public async Task CalculateAdjustmentsAsync(MatchEntity match, CancellationToken cancellationToken = default)
        {
            if (match?.Score == null) return;

            List<MMRAdjustment> adjustments = new();

            List<PlayerMMR> playerMMRs = await GetPlayerMMRsAsync(match, cancellationToken);

            foreach(MatchPlayer matchPlayer in match.HomePlayers)
            {
                adjustments.Add(new(
                    matchPlayer.UserId,
                    match.Score.Outcome == Match.Enums.ScoreOutcome.Home
                    ? 10 :
                    match.Score.Outcome == Match.Enums.ScoreOutcome.Away
                    ? -10 : 0,
                    match));
            }

            foreach (MatchPlayer matchPlayer in match.AwayPlayers)
            {
                adjustments.Add(new(
                    matchPlayer.UserId,
                    match.Score.Outcome == Match.Enums.ScoreOutcome.Away
                    ? 10 :
                    match.Score.Outcome == Match.Enums.ScoreOutcome.Home
                    ? -10 : 0,
                    match));
            }

            ApplyAdjustmentsToPlayerMMRs(adjustments, playerMMRs);
        }

        public decimal CalculateTeamMMRAdjustment(MatchScore score, List<PlayerMMR> homePlayerMMRs, List<PlayerMMR> awayPlayerMMRs)
        {
            decimal homeAvgMMR = homePlayerMMRs.Average(x => x.MMR);
            decimal awayAvgMMR = awayPlayerMMRs.Average(x => x.MMR);

            decimal expectedHome = 1 / (1 + Convert.ToDecimal(Math.Pow(10, (double)(awayAvgMMR - homeAvgMMR) / 400)));

            decimal actualHome = (score.Maps.Sum(map => map.Home) - score.Maps.Sum(map => map.Away)) / 7;

            decimal adjustment = 20 * (actualHome - expectedHome);

            return adjustment;
        }

        private void ApplyAdjustmentsToPlayerMMRs(List<MMRAdjustment> adjustments, List<PlayerMMR> playerMMRs)
        {
            foreach (MMRAdjustment adjustment in adjustments)
            {
                PlayerMMR? playerMMR = playerMMRs.FirstOrDefault(x => x.UserId.Equals(adjustment.UserId));
                if (playerMMR != null)
                {
                    playerMMR.ApplyAdjustment(adjustment);
                }
            }
        }

        protected Task<List<PlayerMMR>> GetPlayerMMRsAsync(MatchEntity match, CancellationToken cancellationToken = default);
    }
}
