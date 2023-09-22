using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Domain.Match.Enums;
using ApacBreachersRanked.Domain.User.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.MatchQueue.Queries
{
    public class IsUserInMatchQuery : IQuery<bool>
    {
        public IUserId UserId { get; set; } = null!;
    }

    public class IsUserInMatchHandler : IQueryHandler<IsUserInMatchQuery, bool>
    {
        private readonly IDbContext _dbContext;

        public IsUserInMatchHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Task<bool> Handle(IsUserInMatchQuery request, CancellationToken cancellationToken)
            => _dbContext.Matches.AnyAsync(
                x => (x.Status == MatchStatus.PendingConfirmation ||
                      x.Status == MatchStatus.Confirmed) &&
                      x.AllPlayers.Any(x => x.UserId.Equals(request.UserId)),
                cancellationToken);
    }
}
