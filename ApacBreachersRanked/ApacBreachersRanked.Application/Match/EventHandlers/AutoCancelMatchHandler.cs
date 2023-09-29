using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.Match.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.Match.EventHandlers
{
    public class AutoCancelMatchHandler : INotificationHandler<AutoCancelMatchEvent>
    {
        private readonly IDbContext _dbContext;
        
        public AutoCancelMatchHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task Handle(AutoCancelMatchEvent notification, CancellationToken cancellationToken)
        {
            MatchEntity match = await _dbContext.Matches
                .Include(x => x.AllPlayers)
                .Where(match => match.Id == notification.MatchId)
                .SingleAsync(cancellationToken);

            match.AutoCancel();

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
