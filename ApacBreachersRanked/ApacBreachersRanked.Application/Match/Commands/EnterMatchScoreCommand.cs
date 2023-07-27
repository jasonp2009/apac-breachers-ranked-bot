using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.Match.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.Match.Commands
{
    public class EnterMatchScoreCommand : ICommand
    {
        public ulong MatchThreadId { get; set; }
        public Map Map { get; set; }
        public int Home { get; set; }
        public int Away { get; set; }
    }

    public class EnterMatchScoreCommandHandler : ICommandHandler<EnterMatchScoreCommand>
    {
        private readonly IDbContext _dbContext;

        public EnterMatchScoreCommandHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(EnterMatchScoreCommand request, CancellationToken cancellationToken)
        {
            MatchEntity match = await _dbContext.MatchThreads
                .Include(x => x.Match)
                .Where(x => x.MatchThreadId == request.MatchThreadId)
                .Select(x => x.Match)
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new InvalidOperationException("Unable to find match");

            match.SetScore(new List<MatchMap>
            {
                new MatchMap(request.Map, new MatchScore(request.Home, request.Away))
            });

            await _dbContext.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
