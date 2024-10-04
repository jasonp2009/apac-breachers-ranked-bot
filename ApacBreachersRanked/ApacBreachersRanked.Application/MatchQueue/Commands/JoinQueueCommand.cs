using ApacBreachersRanked.Application.BreachersUsers.Queries;
using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.MatchQueue.Exceptions;
using ApacBreachersRanked.Application.MatchQueue.Queries;
using ApacBreachersRanked.Application.Moderation.Commands;
using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.Helpers;
using ApacBreachersRanked.Domain.Match.Enums;
using ApacBreachersRanked.Domain.MatchQueue.Entities;
using ApacBreachersRanked.Domain.User.Interfaces;
using MediatR;

namespace ApacBreachersRanked.Application.MatchQueue.Commands
{
    public class JoinQueueCommand : ICommand
    {
        public ulong DiscordUserId { get; set; }
        public int TimeoutMins { get; set; }
        public MatchFormat MatchFormat { get; init; }
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

            IUser user = await _mediator.Send(new GetDiscordUserQuery { DiscordUserId = request.DiscordUserId }, cancellationToken);

            if (await _mediator.Send(new IsUserInMatchQuery { UserId = request.DiscordUserId.ToIUserId() }, cancellationToken))
            {
                throw new UserInMatchException(user);
            }

            await _mediator.Send(new IsUserLinkedQuery
                {
                    DiscordUserId = request.DiscordUserId, ThrowWhenNotLinked = true
                },
                cancellationToken);

            
            foreach (var matchFormat in MatchConstantsExtensions.GetEnabledMatchFormats().Where(format => format != request.MatchFormat))
            {
                MatchQueueEntity otherQueue = await _mediator.Send(new GetCurrentQueueQuery(matchFormat), cancellationToken);

                otherQueue.RemoveUserFromQueue(request.DiscordUserId.ToIUserId());
            }
            
            MatchQueueEntity currentQueue = await _mediator.Send(new GetCurrentQueueQuery(request.MatchFormat), cancellationToken);

            currentQueue.AddUserToQueue(user, DateTime.UtcNow + TimeSpan.FromMinutes(request.TimeoutMins));

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
