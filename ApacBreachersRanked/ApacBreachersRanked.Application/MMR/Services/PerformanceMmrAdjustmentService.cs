using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.MMR.Extensions;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.MatchData.Entities;
using ApacBreachersRanked.Domain.MMR.Entities;
using ApacBreachersRanked.Domain.MMR.Services;
using ApacBreachersRanked.Domain.User.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.MMR.Services;

public class PerformanceMmrAdjustmentService : IMmrAdjustmentService
{
    private const decimal KFactor = 24;
    private const decimal PerfomanceWeighting = 0.3M;
    
    private readonly MmrAdjustmentService _mmrAdjustmentService = new();
    private readonly IDbContext _dbContext;

    public PerformanceMmrAdjustmentService(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<MMRAdjustment>> CalculateAdjustmentsAsync(MatchEntity match, IEnumerable<PlayerMMR> playerMmrs, CancellationToken cancellationToken)
    {
        var playerMmrsList = playerMmrs.ToList();
        var mmrAdjustments = (await _mmrAdjustmentService.CalculateAdjustmentsAsync(match, playerMmrsList, cancellationToken)).ToList();
        var gameData =
            await _dbContext.GameData.FirstOrDefaultAsync(gameData => gameData.MatchId == match.Id, cancellationToken);
        if (gameData is null)
        {
            return mmrAdjustments;
        }

        var kFactor = mmrAdjustments.Sum(adj => Math.Abs(adj.Adjustment)) / mmrAdjustments.Count * 2;
        
        var performanceAdjustments = CalculatePerformanceAdjustments(match, playerMmrsList, gameData, kFactor).ToList();

        var finalAdjustments = new List<MMRAdjustment>();
        foreach (var mmrAdjustment in mmrAdjustments)
        {
            var performanceAdjustment =
                performanceAdjustments.First(pa => pa.UserId.Equals(mmrAdjustment.UserId));
            var finalAdjustment = mmrAdjustment.Adjustment * (1 - PerfomanceWeighting) + performanceAdjustment.Adjustment * PerfomanceWeighting;
            finalAdjustments.Add(new MMRAdjustment(mmrAdjustment.UserId, match.MatchFormat,finalAdjustment, match));
        }

        return finalAdjustments;
    }
    
    private IEnumerable<MMRAdjustment> CalculatePerformanceAdjustments(MatchEntity match, IEnumerable<PlayerMMR> playerMmrs, GameDataEntity gameData, decimal kFactor = KFactor)
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

        foreach (var player in homePlayerMmrs)
            adjustments.Add(new MMRAdjustment(
                player.UserId,
                match.MatchFormat,
                CalculatePlayerMmrAdjustment(player, homePlayerMmrs, gameData, kFactor),
                match));

        foreach (var player in awayPlayerMmrs)
            adjustments.Add(new MMRAdjustment(
                player.UserId,
                match.MatchFormat,
                CalculatePlayerMmrAdjustment(player, awayPlayerMmrs, gameData, kFactor),
                match));

        return adjustments;
    }

    private static decimal CalculatePlayerMmrAdjustment(PlayerMMR playerMmr, List<PlayerMMR> teamMmrs, GameDataEntity gameData, decimal kFactor)
    {
        decimal expected = CalculateExpected(playerMmr, teamMmrs);
        decimal actual = CalculateActual(playerMmr.UserId, gameData);
        return MmrExtensions.CalculateAdjustment(kFactor, expected, actual);
    }
    
    private static decimal CalculateExpected(PlayerMMR playerMmr, List<PlayerMMR> teamMmrs)
    {
        return MmrExtensions.CalculateExpected(playerMmr.MMR, teamMmrs.Average(teamMmr => teamMmr.MMR));
    }

    private static decimal CalculateActual(IUserId player, GameDataEntity gameData)
    {
        var gamePlayerData = gameData.Players.First(gamePlayer => gamePlayer.UserId.Equals(player));
        var teamPlayerData = gameData.Players.Where(gamePlayer => gamePlayer.Side == gamePlayerData.Side);
        return (decimal)gamePlayerData.Score / ((decimal)teamPlayerData.Average(x => x.Score) * 2);
    }
}