using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.MatchQueue.Exceptions;
using ApacBreachersRanked.Application.MatchQueue.Queries;
using ApacBreachersRanked.Application.Moderation.Commands;
using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.MatchQueue.Entities;
using MediatR;

namespace ApacBreachersRanked.Application.MatchQueue.Commands
{
    public class JoinQueueCommand : ICommand
    {
        public ulong DiscordUserId { get; set; }
        public int TimeoutMins { get; set; }
    }

    public class JoinQueueCommandHandler : ICommandHandler<JoinQueueCommand>
    {
        private readonly IMediator _mediator;
        private readonly IDbContext _dbContext;

        public JoinQueueCommandHandler(IMediator mediator, IDbContext dbContext)
        {
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(JoinQueueCommand request, CancellationToken cancellationToken)
        {
            await _mediator.Send(new ThrowIfBannedCommand { UserId = request.DiscordUserId.ToIUserId() }, cancellationToken);

            ApplicationDiscordUser user = await _mediator.Send(new GetDiscordUserQuery() { DiscordUserId = request.DiscordUserId }, cancellationToken);

            MatchEntity? currentMatch = await _mediator.Send(new GetInProgressMatchByUserQuery { UserId = request.DiscordUserId.ToIUserId() }, cancellationToken);

            if (currentMatch != null)
            {
                throw new UserInMatchException(user, currentMatch);
            }

            MatchQueueEntity currentQueue = await _mediator.Send(new GetCurrentQueueQuery(), cancellationToken);

            currentQueue.AddUserToQueue(user, DateTime.UtcNow + TimeSpan.FromMinutes(request.TimeoutMins));

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
