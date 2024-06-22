using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.Match.Services;
using ApacBreachersRanked.Infrastructure.Breachers.Entities;
using ApacBreachersRanked.Infrastructure.Breachers.Models;
using ApacBreachersRanked.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

using DomainMap = ApacBreachersRanked.Domain.Match.Enums.Map;

namespace ApacBreachersRanked.Infrastructure.Breachers.Services;

internal class MatchDataService : IMatchDataService
{
    private readonly BreachersDbContext _dbContext;

    public MatchDataService(BreachersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<MatchScore> GetScore(Guid matchId, CancellationToken cancellationToken)
    {
        (MatchEntity match, MatchDataEntity matchStats) = await GetMatchData(matchId, cancellationToken);
        
        List<BreachersDiscordUserLink> homeUserLinks = await _dbContext.BreachersDiscordUserLinks
            .Where(link => match.HomePlayers.Select(player => player.UserId.GetDiscordId()).Contains(link.DiscordUserId))
            .ToListAsync(cancellationToken);
        List<BreachersDiscordUserLink> awayUserLinks = await _dbContext.BreachersDiscordUserLinks
            .Where(link => match.AwayPlayers.Select(player => player.UserId.GetDiscordId()).Contains(link.DiscordUserId))
            .ToListAsync(cancellationToken);
        MatchScore matchScore = new();
        foreach (GetMatchResponse gameStats in matchStats.Games)
        {
            IEnumerable<BreachersPlayer> homePlayers = gameStats.GameData.AllPlayers.Where(player =>
                homeUserLinks.Select(link => link.BreachersUserId).Contains(player.Id));
            IEnumerable<BreachersPlayer> awayPlayers = gameStats.GameData.AllPlayers.Where(player =>
                awayUserLinks.Select(link => link.BreachersUserId).Contains(player.Id));
            DomainMap map = gameStats.GameData.Map;
            int homeScore = homePlayers.MaxBy(player => player.GameTimeInSeconds).Rounds
                .Count(round => round.Team == round.TeamWon);
            int awayScore = awayPlayers.MaxBy(player => player.GameTimeInSeconds).Rounds
                .Count(round => round.Team == round.TeamWon);
            matchScore.Maps.Add(new(map, homeScore, awayScore));
        }

        return matchScore;
    }

    private async Task<(MatchEntity, MatchDataEntity)> GetMatchData(Guid matchId, CancellationToken cancellationToken)
    {
        MatchEntity match = await _dbContext.Matches
                                .Include(x => x.AllPlayers)
                                .FirstOrDefaultAsync(x => x.Id == matchId, cancellationToken)
                            ?? throw new KeyNotFoundException($"Invalid match id: {matchId}");
        MatchDataEntity matchDatas =
            await _dbContext.MatchStats
                .FirstOrDefaultAsync(x => x.Id == matchId, cancellationToken)
            ?? throw new KeyNotFoundException($"Match stats not ready for match id: {matchId}");
        return (match, matchDatas);
    }
}
