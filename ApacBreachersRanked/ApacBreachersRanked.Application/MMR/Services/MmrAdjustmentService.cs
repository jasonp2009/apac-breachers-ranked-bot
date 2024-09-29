using ApacBreachersRanked.Application.MMR.Extensions;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.Match.Enums;
using ApacBreachersRanked.Domain.MMR.Entities;
using ApacBreachersRanked.Domain.MMR.Services;

namespace ApacBreachersRanked.Application.MMR.Services;

public class MmrAdjustmentService : IMmrAdjustmentService
{
    private static readonly decimal KFactor = 24;
    private static readonly decimal MapWeighting = 0.55M;
    private static readonly decimal RoundWeighting = 0.45M;

    public async Task<IEnumerable<MMRAdjustment>> CalculateAdjustmentsAsync(MatchEntity match, IEnumerable<PlayerMMR> playerMmrs, CancellationToken cancellationToken)
    {
        if (!match.HomePlayers.Any()) throw new ArgumentException(nameof(match.HomePlayers));
        if (!match.AwayPlayers.Any()) throw new ArgumentException(nameof(match.AwayPlayers));
        if (match?.Score is null) throw new ArgumentNullException(nameof(match.Score));

        List<MMRAdjustment> adjustments = new();

        var playerMmrsList = playerMmrs.ToList();

        var homePlayerMmrs =
            playerMmrsList.Where(playerMmr =>
                match.HomePlayers.Any(homePlayer => homePlayer.UserId.Equals(playerMmr.UserId))).ToList();
        var awayPlayerMmrs =
            playerMmrsList.Where(playerMmr =>
                match.AwayPlayers.Any(awayPlayer => awayPlayer.UserId.Equals(playerMmr.UserId))).ToList();

        var homeTeamMmrAdjustment = CalculateTeamMmrAdjustment(match.Score, homePlayerMmrs, awayPlayerMmrs);

        foreach (var player in homePlayerMmrs)
            adjustments.Add(new MMRAdjustment(
                player.UserId,
                match.MatchFormat,
                CalculatePlayerMmrAdjustment(homeTeamMmrAdjustment, homePlayerMmrs, player),
                match));

        foreach (var player in awayPlayerMmrs)
            adjustments.Add(new MMRAdjustment(
                player.UserId,
                match.MatchFormat,
                CalculatePlayerMmrAdjustment(-homeTeamMmrAdjustment, awayPlayerMmrs, player),
                match));

        return adjustments;
    }

    private static decimal CalculateTeamMmrAdjustment(MatchScore score, List<PlayerMMR> homePlayerMmRs,
        List<PlayerMMR> awayPlayerMmRs)
    {
        decimal totalPlayers = homePlayerMmRs.Count + awayPlayerMmRs.Count;
        var homeWeightedAvgMmr = homePlayerMmRs.Sum(x => x.MMR) / (totalPlayers / 2);
        var awayWeightedAvgMmr = awayPlayerMmRs.Sum(x => x.MMR) / (totalPlayers / 2);

        var expectedHome = MmrExtensions.CalculateExpected(homeWeightedAvgMmr, awayWeightedAvgMmr);

        var roundDiff = score.Maps.Sum(map => map.Home) - score.Maps.Sum(map => map.Away);

        var actualHomeRound = (decimal)roundDiff / 14 + (decimal)0.5;

        var actualHomeMap = score.Outcome == ScoreOutcome.Home ? 1
            : score.Outcome == ScoreOutcome.Away ? 0
            : 0.5M;

        var actualHome = actualHomeRound * RoundWeighting + actualHomeMap * MapWeighting;

        var adjustment = MmrExtensions.CalculateAdjustment(KFactor, expectedHome, actualHome) * (homePlayerMmRs.Count + awayPlayerMmRs.Count) / 2 ;

        return adjustment;
    }
    

    private static decimal CalculatePlayerMmrAdjustment(decimal teamMmrAdjustment, List<PlayerMMR> teamMmRs,
        PlayerMMR playerMmr)
    {
        return teamMmrAdjustment / teamMmRs.Count;
    }
}