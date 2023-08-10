using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.Match.Events;
using ApacBreachersRanked.Domain.MMR.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.MMR.EventHandlers
{
    public class AdjustMMRHandler : INotificationHandler<MatchScoreSetEvent>
    {
        private readonly IDbContext _dbContext;
        private readonly IMMRAdjustmentService _mmrAdjustmentService;

        public AdjustMMRHandler(IDbContext dbContext, IMMRAdjustmentService mmrAdjustmentService)
        {
            _dbContext = dbContext;
            _mmrAdjustmentService = mmrAdjustmentService;
        }

        public async Task Handle(MatchScoreSetEvent notification, CancellationToken cancellationToken)
        {
            MatchEntity match = await _dbContext.Matches.FirstAsync(match => match.Id == notification.MatchId, cancellationToken);

            await _mmrAdjustmentService.CalculateAdjustmentsAsync(match, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
