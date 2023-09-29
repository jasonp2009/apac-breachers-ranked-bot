using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.Match.Events;
using ApacBreachersRanked.Domain.MMR.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApacBreachersRanked.Application.MMR.EventHandlers
{
    public class AdjustMMRHandler : INotificationHandler<MatchCompletedEvent>
    {
        private readonly IDbContext _dbContext;
        private readonly IMMRAdjustmentService _mmrAdjustmentService;
        private readonly ILogger<AdjustMMRHandler> _logger;

        public AdjustMMRHandler(IDbContext dbContext, IMMRAdjustmentService mmrAdjustmentService, ILogger<AdjustMMRHandler> logger)
        {
            _dbContext = dbContext;
            _mmrAdjustmentService = mmrAdjustmentService;
            _logger = logger;
        }

        public async Task Handle(MatchCompletedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                MatchEntity match = await _dbContext.Matches
                    .Include(x => x.AllPlayers)
                    .Include(x => x.Score)
                    .Where(match => match.Id == notification.MatchId)
                    .FirstAsync(cancellationToken);

                await _mmrAdjustmentService.CalculateAdjustmentsAsync(match, cancellationToken);

                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred when attempting to calculate the MMR Adjustments for Match {MatchId}", notification.MatchId);
            }
            
        }
    }
}
