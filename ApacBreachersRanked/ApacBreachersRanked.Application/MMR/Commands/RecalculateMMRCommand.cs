using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.MMR.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApacBreachersRanked.Application.MMR.Commands
{
    public class RecalculateMMRCommand : ICommand
    {
    }

    public class RecalculateMMRCommandHandler : ICommandHandler<RecalculateMMRCommand>
    {
        private readonly IDbContext _dbContext;
        private readonly IMMRAdjustmentService _mmrAdjustmentService;
        private readonly ILogger<RecalculateMMRCommand> _logger;

        public RecalculateMMRCommandHandler(IDbContext dbContext, IMMRAdjustmentService mmrAdjustmentService, ILogger<RecalculateMMRCommand> logger)
        {
            _dbContext = dbContext;
            _mmrAdjustmentService = mmrAdjustmentService;
            _logger = logger;
        }

        public async Task<Unit> Handle(RecalculateMMRCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _dbContext.ResetMMRAsync();
                await _dbContext.SaveChangesAsync(cancellationToken);

                List<MatchEntity> matches = await _dbContext.Matches.OrderBy(x => x.MatchNumber).ToListAsync(cancellationToken);

                foreach (MatchEntity match in matches)
                {
                    await _mmrAdjustmentService.CalculateAdjustmentsAsync(match, cancellationToken);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }

                return Unit.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred when trying to recalculate MMR");
                throw;
            }
            
        }
    }
}
