using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.MatchQueue
{
    public class AddUserToQueueCommand : IRequest<Unit>
    {
        public ulong DiscordUserId { get; set; }
        public int TimeoutMins { get; set; }
    }

    public class AddUserToQueueCommandHandler : IRequestHandler<AddUserToQueueCommand, Unit>
    {
        private readonly IMediator _mediator;
        private readonly IMatchQueueDbContext _matchQueueDbContext;

        public AddUserToQueueCommandHandler(IMediator mediator, IMatchQueueDbContext matchQueueDbContext)
        {
            _mediator = mediator;
            _matchQueueDbContext = matchQueueDbContext;
        }

        public async Task<Unit> Handle(AddUserToQueueCommand request, CancellationToken cancellationToken)
        {
            ApplicationDiscordUser user = await _mediator.Send(new GetDiscordUserQuery() { DiscordUserId = request.DiscordUserId }, cancellationToken);
            MatchQueueEntity? currentQueue = await _matchQueueDbContext.MatchQueue.FirstOrDefaultAsync(x => x.IsOpen, cancellationToken);

            if (currentQueue == null)
            {
                currentQueue = new();
                await _matchQueueDbContext.MatchQueue.AddAsync(currentQueue);
            }

            currentQueue.AddUserToQueue(user, DateTime.UtcNow + TimeSpan.FromMinutes(request.TimeoutMins));

            await _matchQueueDbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
