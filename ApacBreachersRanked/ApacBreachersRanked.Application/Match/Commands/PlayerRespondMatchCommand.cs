using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.Match.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.Match.Commands
{
    public class PlayerRespondMatchCommand : ICommand
    {
        public ulong DiscordUserId { get; set; }
        public bool IsAccepted { get; set; }
    }

    public class PlayerConfirmMatchCommandHandler : ICommandHandler<PlayerRespondMatchCommand>
    {
        private readonly IDbContext _dbContext;
        public PlayerConfirmMatchCommandHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Unit> Handle(PlayerRespondMatchCommand request, CancellationToken cancellationToken)
        {
            MatchPlayer? matchPlayer = await _dbContext.MatchPlayers
                .Where(mp =>
                    mp.Match.Status == Domain.Match.Enums.MatchStatus.PendingConfirmation &&
                    mp.UserId == request.DiscordUserId.ToIUserId())
                .FirstOrDefaultAsync(cancellationToken);

            if (matchPlayer == null) return Unit.Value;

            if (request.IsAccepted)
            {
                matchPlayer.Confirm();
            } else
            {
                matchPlayer.Reject();
            }
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
