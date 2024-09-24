using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.MatchQueue.Commands;
using ApacBreachersRanked.Domain.Helpers;
using ApacBreachersRanked.Domain.MatchQueue.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Infrastructure.MatchQueueListener
{
    public class ForceMatchCommandHandler : ICommandHandler<ForceMatchCommand>
    {
        private readonly IDbContext _dbContext;

        public ForceMatchCommandHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(ForceMatchCommand request, CancellationToken cancellationToken)
        {
            MatchQueueEntity? matchQueue = await _dbContext.MatchQueue
                .Include(x => x.Users)
                .Where(x => x.IsOpen)
                .FirstOrDefaultAsync(cancellationToken);
            if (matchQueue == null) return Unit.Value;

            var matchConstants = matchQueue.MatchFormat.GetMatchFormatConstants();
            
            if (matchQueue.Users.Count < matchConstants.MinCapacity) throw new InvalidOperationException($"There must be atleast {matchConstants.MinCapacity} players to force match");

            return Unit.Value;
        }
    }
}
