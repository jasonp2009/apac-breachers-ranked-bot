using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.Match.Models;
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
                .ThenInclude(x => x.AllPlayers)
                .Where(x => x.MatchThreadId == request.MatchThreadId)
                .Select(x => x.Match)
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new InvalidOperationException("Unable to find match");

            MatchScore score = new();
            score.Maps.Add(new MapScore(request.Map, request.Home, request.Away));

            PendingMatchScore pendingMatchScore = new(match, score);

            _dbContext.PendingMatchScores.Add(pendingMatchScore);

            await _dbContext.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
