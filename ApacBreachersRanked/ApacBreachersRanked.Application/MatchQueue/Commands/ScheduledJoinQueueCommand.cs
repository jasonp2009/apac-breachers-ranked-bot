using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.MatchQueue.Queries;
using ApacBreachersRanked.Application.Moderation.Commands;
using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.MatchQueue.Entities;
using ApacBreachersRanked.Domain.User.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.MatchQueue.Commands
{
    public class ScheduledJoinQueueCommand : ICommand
    {
        public ulong DiscordUserId { get; set; }
        public DateTime JoinAtUtc { get; set; }
    }

    public class ScheduledJoinQueueByIdCommand : ICommand
    {
        public ulong DiscordUserId { get; set; }
        public Guid ScheduledQueueId { get; set; }
    }

    public class ScheduledJoinQueueValidator : AbstractValidator<ScheduledJoinQueueCommand>
    {
        public ScheduledJoinQueueValidator()
        {
            RuleFor(command => command.JoinAtUtc)
                .Must(joinAtUtc => joinAtUtc.Millisecond == 0 &&
                                   joinAtUtc.Second == 0 &&
                                   joinAtUtc.Minute % 15 == 0)
                .WithMessage("Join time must be at a 15 minute interval");
        }
    }

    public class ScheduledJoinQueueCommandHandler
        : ICommandHandler<ScheduledJoinQueueCommand>,
            ICommandHandler<ScheduledJoinQueueByIdCommand>
    {
        private readonly IMediator _mediator;
        private readonly IDbContext _dbContext;

        public ScheduledJoinQueueCommandHandler(
            IMediator mediator,
            IDbContext dbContext)
        {
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(ScheduledJoinQueueCommand request, CancellationToken cancellationToken)
        {
            await _mediator.Send(new ThrowIfBannedCommand { UserId = request.DiscordUserId.ToIUserId() }, cancellationToken);

            IUser user = await _mediator.Send(new GetDiscordUserQuery { DiscordUserId = request.DiscordUserId }, cancellationToken);

            ScheduledMatchQueueEntity currentQueue = await _mediator.Send(new GetScheduleQueueQuery { ScheduledForUtc = request.JoinAtUtc }, cancellationToken);

            currentQueue.AddUserToQueue(user);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

        public async Task<Unit> Handle(ScheduledJoinQueueByIdCommand request, CancellationToken cancellationToken)
        {
            IUser user = await _mediator.Send(new GetDiscordUserQuery { DiscordUserId = request.DiscordUserId }, cancellationToken);

            ScheduledMatchQueueEntity scheduledQueue = await _mediator.Send(
                new GetScheduledQueueByIdQuery() { ScheduledMatchQueueId = request.ScheduledQueueId },
                cancellationToken);

            if (scheduledQueue == null) return Unit.Value;
            
            scheduledQueue.AddUserToQueue(user);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
