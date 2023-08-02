using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.MMR.Entities;

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
