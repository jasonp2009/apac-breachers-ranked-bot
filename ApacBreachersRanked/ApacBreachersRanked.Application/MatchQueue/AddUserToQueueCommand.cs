using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.Entities;
using ApacBreachersRanked.Domain.Repositories;
using MediatR;

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
        private readonly IMatchQueueRepository _matchQueueRepository;

        public AddUserToQueueCommandHandler(IMediator mediator, IMatchQueueRepository matchQueueRepository)
        {
            _mediator = mediator;
            _matchQueueRepository = matchQueueRepository;
        }

        public async Task<Unit> Handle(AddUserToQueueCommand request, CancellationToken cancellationToken)
        {
            DiscordUser user = await _mediator.Send(new GetDiscordUserQuery() { DiscordUserId = request.DiscordUserId }, cancellationToken);
            MatchQueueEntity currentQueue = _matchQueueRepository.CurrentQueue ?? new();

            currentQueue.AddUserToQueue(user, DateTime.UtcNow + TimeSpan.FromMinutes(request.TimeoutMins));

            await _matchQueueRepository.SaveAsync(currentQueue, cancellationToken);

            return Unit.Value;
        }
    }
}
