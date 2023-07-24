using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.MatchQueue.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.MatchQueue.Commands
{
    public class AddUserToQueueCommand : ICommand
    {
        public ulong DiscordUserId { get; set; }
        public int TimeoutMins { get; set; }
    }

    public class AddUserToQueueCommandHandler : ICommandHandler<AddUserToQueueCommand>
    {
        private readonly IMediator _mediator;
        private readonly IDbContext _dbContext;

        public AddUserToQueueCommandHandler(IMediator mediator, IDbContext dbContext)
        {
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(AddUserToQueueCommand request, CancellationToken cancellationToken)
        {
            ApplicationDiscordUser user = await _mediator.Send(new GetDiscordUserQuery() { DiscordUserId = request.DiscordUserId }, cancellationToken);
            MatchQueueEntity? currentQueue = await _dbContext.MatchQueue.FirstOrDefaultAsync(x => x.IsOpen, cancellationToken);

            if (currentQueue == null)
            {
                currentQueue = new();
                await _dbContext.MatchQueue.AddAsync(currentQueue);
            }

            currentQueue.AddUserToQueue(user, DateTime.UtcNow + TimeSpan.FromMinutes(request.TimeoutMins));

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
