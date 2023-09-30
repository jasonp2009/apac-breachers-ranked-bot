using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.Match.Models;
using ApacBreachersRanked.Application.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.Match.Commands
{
    public class PlayerRespondScoreCommand : ICommand
    {
        public bool IsConfirm { get; set; }
        public ulong PendingScoreMessageId { get; set; }
        public ulong UserId { get; set; }
    }

    public class PlayerRespondScoreCommandHandler : ICommandHandler<PlayerRespondScoreCommand>
    {
        private readonly IDbContext _dbContext;

        public PlayerRespondScoreCommandHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(PlayerRespondScoreCommand request, CancellationToken cancellationToken)
        {
            PendingMatchScore pendingMatchScore = await _dbContext.PendingMatchScores
                .Include(x => x.Players)
                .Where(x => x.MessageId == request.PendingScoreMessageId)
                .SingleAsync(cancellationToken);

            pendingMatchScore.SetPlayerConfirmationStatus(request.UserId.ToIUserId(), request.IsConfirm);

            await _dbContext.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
