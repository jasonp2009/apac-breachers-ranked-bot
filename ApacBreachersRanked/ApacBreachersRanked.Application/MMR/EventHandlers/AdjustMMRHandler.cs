using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.MMR.Extensions;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.Match.Events;
using ApacBreachersRanked.Domain.MMR.Events;
using ApacBreachersRanked.Domain.MMR.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApacBreachersRanked.Application.MMR.EventHandlers
{
    public class AdjustMMRHandler : INotificationHandler<MatchCompletedEvent>
    {
        private readonly IDbContext _dbContext;
        private readonly IMmrService _mmrService;
        private readonly IMmrAdjustmentService _mmrAdjustmentService;
        private readonly ILogger<AdjustMMRHandler> _logger;

        public AdjustMMRHandler(IDbContext dbContext, IMmrAdjustmentService mmrAdjustmentService, ILogger<AdjustMMRHandler> logger, IMmrService mmrService)
        {
            _dbContext = dbContext;
            _mmrAdjustmentService = mmrAdjustmentService;
            _logger = logger;
            _mmrService = mmrService;
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
                var allPlayerMmrs =
                    await _mmrService.GetPlayerMmRsAsync(match.AllPlayers, match.MatchFormat, cancellationToken);
                var adjustments = _mmrAdjustmentService.CalculateAdjustments(match,allPlayerMmrs).ToList();

                allPlayerMmrs.ApplyAdjustmentsToPlayerMmrs(adjustments);
                match.QueueDomainEvent(new MatchMMRCalculatedEvent { MatchId = match.Id });

                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred when attempting to calculate the MMR Adjustments for Match {MatchId}", notification.MatchId);
            }
            
        }
    }
}
