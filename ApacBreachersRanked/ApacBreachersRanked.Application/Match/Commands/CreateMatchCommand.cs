using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.Match.Services;
using ApacBreachersRanked.Domain.MatchQueue.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApacBreachersRanked.Application.Match.Commands
{
    public class CreateMatchCommand : ICommand
    {

    }
    public class CreateMatchCommandHandler : ICommandHandler<CreateMatchCommand>
    {
        private readonly IDbContext _dbContext;
        private readonly IMatchService _matchService;
        private readonly ILogger<CreateMatchCommandHandler> _logger;
        public CreateMatchCommandHandler(
            IDbContext dbContext,
            IMatchService matchService,
            ILogger<CreateMatchCommandHandler> logger)
        {
            _dbContext = dbContext;
            _matchService = matchService;
            _logger = logger;
        }

        public async Task<Unit> Handle(CreateMatchCommand notification, CancellationToken cancellationToken)
        {
            MatchQueueEntity? matchQueue = await _dbContext.MatchQueue
                .Include(x => x.Users)
                .Where(x => x.IsOpen)
                .FirstOrDefaultAsync(cancellationToken);
            if (matchQueue == null || !matchQueue.IsOpen) return Unit.Value;

            try
            {
                MatchEntity match = await _matchService.CreateMatchFromQueueAsync(matchQueue, cancellationToken);

                _dbContext.Matches.Add(match);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An exception occurred when trying to create a match");
            }
            return Unit.Value;
        }
    }
}
