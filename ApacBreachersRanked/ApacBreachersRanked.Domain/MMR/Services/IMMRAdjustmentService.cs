using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.Match.Enums;
using ApacBreachersRanked.Domain.MMR.Entities;
using ApacBreachersRanked.Domain.MMR.Events;
using ApacBreachersRanked.Domain.User.Interfaces;

namespace ApacBreachersRanked.Domain.MMR.Services;

public interface IMMRAdjustmentService
{
    private static readonly decimal KFactor = 24;
    private static readonly decimal MapWeighting = 0.55M;
    private static readonly decimal RoundWeighting = 0.45M;

    public async Task CalculateAdjustmentsAsync(MatchEntity match, CancellationToken cancellationToken = default)
    {
        if (match.HomePlayers.Count() == 0 || match.AwayPlayers.Count() == 0 || match?.Score == null) return;

        List<MMRAdjustment> adjustments = new();

        var homePlayerMMRs = await GetPlayerMMRsAsync(match.HomePlayers, match.MatchFormat, cancellationToken);
        var awayPlayerMMRs = await GetPlayerMMRsAsync(match.AwayPlayers, match.MatchFormat, cancellationToken);

        var homeTeamMMRAdjustment = CalculateTeamMMRAdjustment(match.Score, homePlayerMMRs, awayPlayerMMRs);

        foreach (var player in homePlayerMMRs)
            adjustments.Add(new MMRAdjustment(
                player.UserId,
                match.MatchFormat,
                CalculatePlayerMMRAdjustment(homeTeamMMRAdjustment, homePlayerMMRs, player),
                match));

        foreach (var player in awayPlayerMMRs)
            adjustments.Add(new MMRAdjustment(
                player.UserId,
                match.MatchFormat,
                CalculatePlayerMMRAdjustment(-homeTeamMMRAdjustment, awayPlayerMMRs, player),
                match));

        List<PlayerMMR> allPlayerMMRs = new();
        allPlayerMMRs.AddRange(homePlayerMMRs);
        allPlayerMMRs.AddRange(awayPlayerMMRs);

        ApplyAdjustmentsToPlayerMMRs(adjustments, allPlayerMMRs);
        match.QueueDomainEvent(new MatchMMRCalculatedEvent { MatchId = match.Id });
    }

    private decimal CalculateTeamMMRAdjustment(MatchScore score, List<PlayerMMR> homePlayerMMRs,
        List<PlayerMMR> awayPlayerMMRs)
    {
        decimal totalPlayers = homePlayerMMRs.Count + awayPlayerMMRs.Count;
        var homeWeightedAvgMMR = homePlayerMMRs.Sum(x => x.MMR) / (totalPlayers / 2);
        var awayWeightedAvgMMR = awayPlayerMMRs.Sum(x => x.MMR) / (totalPlayers / 2);

        var expectedHome =
            1 / (1 + Convert.ToDecimal(Math.Pow(10, (double)(awayWeightedAvgMMR - homeWeightedAvgMMR) / 400)));

        var roundDiff = score.Maps.Sum(map => map.Home) - score.Maps.Sum(map => map.Away);

        var actualHomeRound = (decimal)roundDiff / 14 + (decimal)0.5;

        var actualHomeMap = score.Outcome == ScoreOutcome.Home ? 1
            : score.Outcome == ScoreOutcome.Away ? 0
            : 0.5M;

        var actualHome = actualHomeRound * RoundWeighting + actualHomeMap * MapWeighting;

        var adjustment = KFactor * (homePlayerMMRs.Count + awayPlayerMMRs.Count) / 2 * (actualHome - expectedHome);

        return adjustment;
    }

    private decimal CalculatePlayerMMRAdjustment(decimal teamMMRAdjustment, List<PlayerMMR> teamMMRs,
        PlayerMMR playerMMR)
    {
        return teamMMRAdjustment / teamMMRs.Count * (playerMMR.MMR / teamMMRs.Average(x => x.MMR));
    }

    private void ApplyAdjustmentsToPlayerMMRs(List<MMRAdjustment> adjustments, List<PlayerMMR> playerMMRs)
    {
        foreach (var adjustment in adjustments)
        {
            var playerMMR = playerMMRs.FirstOrDefault(x => x.UserId.Equals(adjustment.UserId));
            if (playerMMR != null) playerMMR.ApplyAdjustment(adjustment);
        }
    }

    protected Task<List<PlayerMMR>> GetPlayerMMRsAsync(IEnumerable<IUser> users, MatchFormat matchFormat,
        CancellationToken cancellationToken = default);
}