using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.MatchData.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.Match.EventHandlers;

public class GameDataReadyHandler : INotificationHandler<GameDataReadyEvent>
{
    private readonly IDbContext _dbContext;

    public GameDataReadyHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(GameDataReadyEvent notification, CancellationToken cancellationToken)
    {
        var mapScores = (await _dbContext.GameData
                .Where(x => x.MatchId == notification.MatchId)
                .ToListAsync(cancellationToken))
            .Select(x => x.Score)
            .ToList();
        MatchScore score = new();
        foreach (var mapScore in mapScores) score.Maps.Add(mapScore);
        var match =
            await _dbContext.Matches
                .Include(match => match.AllPlayers)
                .SingleAsync(match => match.Id == notification.MatchId, cancellationToken);

        match.SetScore(score);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}