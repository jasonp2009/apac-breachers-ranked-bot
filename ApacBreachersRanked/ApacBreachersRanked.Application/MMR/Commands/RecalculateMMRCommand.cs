using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.MMR.Entities;
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

                List<MatchEntity> matches = await _dbContext.Matches
                    .Where(x => x.Status == Domain.Match.Enums.MatchStatus.Completed)
                    .OrderBy(x => x.MatchNumber)
                    .ToListAsync(cancellationToken);

                foreach (MatchEntity match in matches)
                {
                    foreach (MatchPlayer player in match.AllPlayers)
                    {
                        PlayerMMR? playerMMR = await _dbContext.PlayerMMRs
                            .Where(x => x.UserId.Equals(player.UserId))
                            .FirstOrDefaultAsync(cancellationToken);
                        player.SetMMR(playerMMR?.MMR ?? 1000);
                        player.SetRank(playerMMR?.Rank);
                    }
                    await _mmrAdjustmentService.CalculateAdjustmentsAsync(match, cancellationToken);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                    await Task.Delay(5000);
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
