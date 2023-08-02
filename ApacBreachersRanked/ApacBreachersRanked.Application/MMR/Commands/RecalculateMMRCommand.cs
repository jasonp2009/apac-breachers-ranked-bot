using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.MMR.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.MMR.Commands
{
    public class RecalculateMMRCommand : ICommand
    {
    }

    public class RecalculateMMRCommandHandler : ICommandHandler<RecalculateMMRCommand>
    {
        private readonly IDbContext _dbContext;
        private readonly IMMRAdjustmentService _mmrAdjustmentService;

        public RecalculateMMRCommandHandler(IDbContext dbContext, IMMRAdjustmentService mmrAdjustmentService)
        {
            _dbContext = dbContext;
            _mmrAdjustmentService = mmrAdjustmentService;
        }

        public async Task<Unit> Handle(RecalculateMMRCommand request, CancellationToken cancellationToken)
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
    }
}
