using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.Moderation.Models;
using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.User.Interfaces;
using MediatR;

namespace ApacBreachersRanked.Application.Moderation.Commands
{
    public class BanUserCommand : ICommand
    {
        public ulong DiscordUserId { get; set; }
        public TimeSpan Duration { get; set; }
        public string Reason { get; set; } = null!;
    }

    public class BanUserHandler : ICommandHandler<BanUserCommand>
    {
        private readonly IDbContext _dbContext;
        private readonly IMediator _mediator;

        public BanUserHandler(IDbContext dbContext, IMediator mediator)
        {
            _dbContext = dbContext;
            _mediator = mediator;
        }
        public async Task<Unit> Handle(BanUserCommand request, CancellationToken cancellationToken)
        {
            IUser user = await _mediator.Send(new GetDiscordUserQuery { DiscordUserId = request.DiscordUserId }, cancellationToken);

            UserBan ban = new(user, request.Duration, request.Reason);

            await _dbContext.UserBans.AddAsync(ban, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
