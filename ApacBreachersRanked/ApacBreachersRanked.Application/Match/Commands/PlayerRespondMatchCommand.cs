using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.Moderation.Commands;
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
        private readonly IMediator _mediator;
        public PlayerConfirmMatchCommandHandler(IDbContext dbContext, IMediator mediator)
        {
            _dbContext = dbContext;
            _mediator = mediator;
        }
        public async Task<Unit> Handle(PlayerRespondMatchCommand request, CancellationToken cancellationToken)
        {
            MatchPlayer? matchPlayer = await _dbContext.MatchPlayers
                .Include(x => x.Match)
                .FirstOrDefaultAsync(mp =>
                    mp.Match.Status == Domain.Match.Enums.MatchStatus.PendingConfirmation &&
                    mp.UserId == request.DiscordUserId.ToIUserId(),
                    cancellationToken);

            if (matchPlayer == null) return Unit.Value;

            if (request.IsAccepted)
            {
                matchPlayer.Confirm();
            } else
            {
                matchPlayer.Reject();
                await _mediator.Send(new BanUserCommand
                {
                    DiscordUserId = matchPlayer.UserId.GetDiscordId(),
                    Duration = TimeSpan.FromMinutes(15),
                    Reason = $"Rejected match #{matchPlayer.Match.MatchNumber}"
                });
            }
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
