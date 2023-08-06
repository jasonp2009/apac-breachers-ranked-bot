using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.MatchQueue.Commands;
using ApacBreachersRanked.Domain.Match.Constants;
using ApacBreachersRanked.Domain.MatchQueue.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Infrastructure.MatchQueueListener
{
    public class ForceMatchCommandHandler : ICommandHandler<ForceMatchCommand>
    {
        private readonly IDbContext _dbContext;
        private readonly MatchQueueListenerService _matchQueueListenerService;

        public ForceMatchCommandHandler(IDbContext dbContext, MatchQueueListenerService matchQueueListenerService)
        {
            _dbContext = dbContext;
            _matchQueueListenerService = matchQueueListenerService;
        }

        public async Task<Unit> Handle(ForceMatchCommand request, CancellationToken cancellationToken)
        {
            MatchQueueEntity? matchQueue = await _dbContext.MatchQueue.FirstOrDefaultAsync(x => x.IsOpen, cancellationToken);
            if (matchQueue == null) return Unit.Value;

            if (matchQueue.Users.Count < MatchConstants.MinCapacity) throw new InvalidOperationException($"There must be atleast {MatchConstants.MinCapacity} players to force match");

            _matchQueueListenerService.ForceStart();

            return Unit.Value;
        }
    }
}
