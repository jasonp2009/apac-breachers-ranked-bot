using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.Match.Models;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.Match.Events;
using ApacBreachersRanked.Domain.Match.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.Match.EventHandlers;

public class MatchDataReadyHandler : INotificationHandler<MatchDataReadyEvent>
{
    private readonly IMatchDataService _matchDataService;
    private readonly IDbContext _dbContext;

    public MatchDataReadyHandler(IMatchDataService matchDataService, IDbContext dbContext)
    {
        _matchDataService = matchDataService;
        _dbContext = dbContext;
    }

    public async Task Handle(MatchDataReadyEvent notification, CancellationToken cancellationToken)
    {
        MatchScore score = await _matchDataService.GetScore(notification.MatchId, cancellationToken);
        MatchEntity match =
            await _dbContext.Matches
                .Include(match => match.AllPlayers)
                .SingleAsync(match => match.Id == notification.MatchId, cancellationToken);
        PendingMatchScore pendingMatchScore = new(match, score);

        _dbContext.PendingMatchScores.Add(pendingMatchScore);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
