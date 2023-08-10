using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.Match.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.Match.EventHandlers
{
    public class PendingScoreConfirmedHandler : INotificationHandler<PendingScoreConfirmedEvent>
    {
        private readonly IDbContext _dbContext;
        
        public PendingScoreConfirmedHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task Handle(PendingScoreConfirmedEvent notification, CancellationToken cancellationToken)
        {
            PendingMatchScore pendingMatchScore = await _dbContext.PendingMatchScores
                .Include(x => x.Match)
                .SingleAsync(x => x.Id == notification.PendingMatchScoreId, cancellationToken);

            pendingMatchScore.Match.SetScore(pendingMatchScore.Score);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
