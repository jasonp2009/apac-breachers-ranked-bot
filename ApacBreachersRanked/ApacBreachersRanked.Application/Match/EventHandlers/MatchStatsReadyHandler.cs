using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.Match.Models;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.Match.Events;
using ApacBreachersRanked.Domain.Match.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.Match.EventHandlers;

public class MatchStatsReadyHandler : INotificationHandler<MatchStatsReadyEvent>
{
    private readonly IMatchStatService _matchStatService;
    private readonly IDbContext _dbContext;

    public MatchStatsReadyHandler(IMatchStatService matchStatService, IDbContext dbContext)
    {
        _matchStatService = matchStatService;
        _dbContext = dbContext;
    }

    public async Task Handle(MatchStatsReadyEvent notification, CancellationToken cancellationToken)
    {
        MatchScore score = await _matchStatService.GetScore(notification.MatchId, cancellationToken);
        MatchEntity match =
            await _dbContext.Matches.SingleAsync(match => match.Id == notification.MatchId, cancellationToken);
        PendingMatchScore pendingMatchScore = new(match, score);

        _dbContext.PendingMatchScores.Add(pendingMatchScore);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
