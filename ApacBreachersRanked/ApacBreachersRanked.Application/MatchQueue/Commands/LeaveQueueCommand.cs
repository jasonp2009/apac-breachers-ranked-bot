using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.MatchQueue.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.MatchQueue.Commands
{
    public class LeaveQueueCommand : ICommand
    {
        public ulong DiscordUserId { get; set; }
    }

    public class LeaveQueueCommandHandler : ICommandHandler<LeaveQueueCommand>
    {
        private readonly IMediator _mediator;
        private readonly IDbContext _dbContext;

        public LeaveQueueCommandHandler(IMediator mediator, IDbContext dbContext)
        {
            _mediator = mediator;
            _dbContext = dbContext;
        }
        public async Task<Unit> Handle(LeaveQueueCommand request, CancellationToken cancellationToken)
        {
            ApplicationDiscordUser user = await _mediator.Send(new GetDiscordUserQuery() { DiscordUserId = request.DiscordUserId }, cancellationToken);
            MatchQueueEntity? currentQueue = await _dbContext.MatchQueue.FirstOrDefaultAsync(x => x.IsOpen, cancellationToken);

            if (currentQueue == null)
            {
                return Unit.Value;
            }

            currentQueue.RemoveUserFromQueue(user);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
