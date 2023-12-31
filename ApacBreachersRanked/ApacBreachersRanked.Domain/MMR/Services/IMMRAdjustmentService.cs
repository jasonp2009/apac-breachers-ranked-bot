﻿using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.MMR.Entities;
using ApacBreachersRanked.Domain.MMR.Events;
using ApacBreachersRanked.Domain.User.Interfaces;

namespace ApacBreachersRanked.Domain.MMR.Services
{
    public interface IMMRAdjustmentService
    {
        private static decimal KFactor = 24;
        private static decimal MapWeighting = 0.55M;
        private static decimal RoundWeighting = 0.45M;
        public async Task CalculateAdjustmentsAsync(MatchEntity match, CancellationToken cancellationToken = default)
        {
            if (match.HomePlayers.Count() == 0 || match.AwayPlayers.Count() == 0 || match?.Score == null) return;

            List<MMRAdjustment> adjustments = new();

            List<PlayerMMR> homePlayerMMRs = await GetPlayerMMRsAsync(match.HomePlayers, cancellationToken);
            List<PlayerMMR> awayPlayerMMRs = await GetPlayerMMRsAsync(match.AwayPlayers, cancellationToken);

            decimal homeTeamMMRAdjustment = CalculateTeamMMRAdjustment(match.Score, homePlayerMMRs, awayPlayerMMRs);

            foreach (PlayerMMR player in homePlayerMMRs)
            {
                adjustments.Add(new(
                    player.UserId,
                    CalculatePlayerMMRAdjustment(homeTeamMMRAdjustment, homePlayerMMRs, player),
                    match));
            }

            foreach (PlayerMMR player in awayPlayerMMRs)
            {
                adjustments.Add(new(
                    player.UserId,
                    CalculatePlayerMMRAdjustment(-homeTeamMMRAdjustment, awayPlayerMMRs, player),
                    match));
            }

            List<PlayerMMR> allPlayerMMRs = new();
            allPlayerMMRs.AddRange(homePlayerMMRs);
            allPlayerMMRs.AddRange(awayPlayerMMRs);

            ApplyAdjustmentsToPlayerMMRs(adjustments, allPlayerMMRs);
            match.QueueDomainEvent(new MatchMMRCalculatedEvent { MatchId = match.Id });
        }

        private decimal CalculateTeamMMRAdjustment(MatchScore score, List<PlayerMMR> homePlayerMMRs, List<PlayerMMR> awayPlayerMMRs)
        {
            decimal homeAvgMMR = homePlayerMMRs.Average(x => x.MMR);
            decimal awayAvgMMR = awayPlayerMMRs.Average(x => x.MMR);

            decimal expectedHome = 1 / (1 + Convert.ToDecimal(Math.Pow(10, (double)(awayAvgMMR - homeAvgMMR) / 400)));

            int roundDiff = score.Maps.Sum(map => map.Home) - score.Maps.Sum(map => map.Away);

            decimal actualHomeRound = (decimal)roundDiff / 14 + (decimal)0.5;

            decimal actualHomeMap = score.Outcome == Match.Enums.ScoreOutcome.Home ? 1
                : score.Outcome == Match.Enums.ScoreOutcome.Away ? 0
                : 0.5M;

            decimal actualHome = actualHomeRound * RoundWeighting + actualHomeMap * MapWeighting;

            decimal adjustment = KFactor * (homePlayerMMRs.Count + awayPlayerMMRs.Count)/2 * (actualHome - expectedHome);

            return adjustment;
        }

        private decimal CalculatePlayerMMRAdjustment(decimal teamMMRAdjustment, List<PlayerMMR> teamMMRs, PlayerMMR playerMMR)
            => (teamMMRAdjustment / teamMMRs.Count) * (playerMMR.MMR / teamMMRs.Average(x => x.MMR));

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

        protected Task<List<PlayerMMR>> GetPlayerMMRsAsync(IEnumerable<IUser> users, CancellationToken cancellationToken = default);
    }
}
