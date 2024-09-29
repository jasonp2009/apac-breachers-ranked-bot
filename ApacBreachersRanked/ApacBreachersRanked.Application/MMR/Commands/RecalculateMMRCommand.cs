using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.MMR.Extensions;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.MMR.Entities;
using ApacBreachersRanked.Domain.MMR.Events;
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
        private readonly IMmrService _mmrService;
        private readonly IMmrAdjustmentService _mmrAdjustmentService;
        private readonly ILogger<RecalculateMMRCommand> _logger;

        public RecalculateMMRCommandHandler(IDbContext dbContext, IMmrAdjustmentService mmrAdjustmentService, ILogger<RecalculateMMRCommand> logger, IMmrService mmrService)
        {
            _dbContext = dbContext;
            _mmrAdjustmentService = mmrAdjustmentService;
            _logger = logger;
            _mmrService = mmrService;
        }

        public async Task<Unit> Handle(RecalculateMMRCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _dbContext.ResetMMRAsync();
                await _dbContext.SaveChangesAsync(cancellationToken);

                List<MatchEntity> matches = await _dbContext.Matches
                    .Include(x => x.AllPlayers)
                    .Include(x => x.Score)
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
                    var allPlayerMmrs =
                        await _mmrService.GetPlayerMmRsAsync(match.AllPlayers, match.MatchFormat, cancellationToken);
                    var adjustments = _mmrAdjustmentService.CalculateAdjustments(match,allPlayerMmrs).ToList();

                    allPlayerMmrs.ApplyAdjustmentsToPlayerMmrs(adjustments);
                    match.QueueDomainEvent(new MatchMMRCalculatedEvent { MatchId = match.Id });
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
