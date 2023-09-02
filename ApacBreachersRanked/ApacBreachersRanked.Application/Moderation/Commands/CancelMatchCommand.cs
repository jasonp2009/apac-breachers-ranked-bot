using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Domain.Match.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.Moderation.Commands
{
    public class CancelMatchCommand : ICommand
    {
        public int MatchNumber { get; set; }
        public string CancellationReason { get; set; } = null!;
    }

    public class CancelMatchHandler : ICommandHandler<CancelMatchCommand>
    {
        private readonly IDbContext _dbContext;

        public CancelMatchHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Unit> Handle(CancelMatchCommand request, CancellationToken cancellationToken)
        {
            MatchEntity match = await _dbContext.Matches.FirstAsync(x => x.MatchNumber == request.MatchNumber, cancellationToken);
            match.CancelMatch(request.CancellationReason);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
