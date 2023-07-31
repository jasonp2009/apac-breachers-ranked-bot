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
        private readonly IDbContext _dbContext;

        public LeaveQueueCommandHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Unit> Handle(LeaveQueueCommand request, CancellationToken cancellationToken)
        {
            MatchQueueEntity? currentQueue = await _dbContext.MatchQueue.FirstOrDefaultAsync(x => x.IsOpen, cancellationToken);

            if (currentQueue == null)
            {
                return Unit.Value;
            }

            currentQueue.RemoveUserFromQueue(request.DiscordUserId.ToIUserId());

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
