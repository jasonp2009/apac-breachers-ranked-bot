using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.Match.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.Match.Commands
{
    public class PlayerConfirmMatchCommand : ICommand
    {
        public ulong DiscordUserId { get; set; }
    }

    public class PlayerConfirmMatchCommandHandler : ICommandHandler<PlayerConfirmMatchCommand>
    {
        private readonly IDbContext _dbContext;
        public PlayerConfirmMatchCommandHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Unit> Handle(PlayerConfirmMatchCommand request, CancellationToken cancellationToken)
        {
            MatchPlayer? matchPlayer = await _dbContext.MatchPlayers
                .FirstOrDefaultAsync(mp =>
                    mp.Match.Status == Domain.Match.Enums.MatchStatus.PendingConfirmation &&
                    mp.UserId == request.DiscordUserId.ToIUserId(),
                    cancellationToken);

            if (matchPlayer == null) return Unit.Value;

            matchPlayer.Confirm();
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
