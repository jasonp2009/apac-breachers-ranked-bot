using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.MatchVote.Models;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.Match.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.MatchVote.EventHandlers
{
    public class StartMatchVotingHandler : INotificationHandler<MatchConfirmedEvent>
    {
        private readonly IDbContext _dbContext;

        public StartMatchVotingHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task Handle(MatchConfirmedEvent notification, CancellationToken cancellationToken)
        {
            MatchEntity match = await _dbContext.Matches
                .Where(x => x.Id == notification.MatchId)
                .SingleAsync(cancellationToken);

            MatchVoteModel matchVote = new(match);

            _dbContext.MatchVotes.Add(matchVote);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
