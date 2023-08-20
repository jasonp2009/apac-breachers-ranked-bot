using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.MatchVote.Events;
using ApacBreachersRanked.Application.MatchVote.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.MatchVote.EventHandlers
{
    public class SideVoteTimedOutHandler : INotificationHandler<SideVoteTimedOutEvent>
    {
        private readonly IDbContext _dbContext;

        public SideVoteTimedOutHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task Handle(SideVoteTimedOutEvent notification, CancellationToken cancellationToken)
        {
            MatchVoteModel matchVote = await _dbContext.MatchVotes.SingleAsync(x => x.MatchId == notification.MatchId, cancellationToken);

            matchVote.CompletSideVote();

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
