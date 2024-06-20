using ApacBreachersRanked.Application.MatchVote.Events;
using ApacBreachersRanked.Infrastructure.Breachers.Events;
using MediatR;

namespace ApacBreachersRanked.Infrastructure.Breachers.EventHandlers;

public class BeginPollingForMatchDataHandler : INotificationHandler<SideVoteCompletedEvent>
{
    private readonly IMediator _mediator;

    public BeginPollingForMatchDataHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(SideVoteCompletedEvent notification, CancellationToken cancellationToken)
    {
        PollForMatchDataEvent pollEvent = new()
        {
            MatchId = notification.MatchId,
            ScheduledForUtc = DateTime.UtcNow + TimeSpan.FromMinutes(30)
        };
        await _mediator.Publish(pollEvent, cancellationToken);
    }
}
