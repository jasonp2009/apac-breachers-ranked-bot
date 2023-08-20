using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.MatchVote.Events;
using ApacBreachersRanked.Application.MatchVote.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.MatchVote.EventHandlers
{
    public class MapVoteTimedOutHandler : INotificationHandler<MapVoteTimedOutEvent>
    {
        private readonly IDbContext _dbContext;
        
        public MapVoteTimedOutHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task Handle(MapVoteTimedOutEvent notification, CancellationToken cancellationToken)
        {
            MatchVoteModel matchVote = await _dbContext.MatchVotes.SingleAsync(x => x.MatchId == notification.MatchId, cancellationToken);

            matchVote.CompletMapVote();

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
