using ApacBreachersRanked.Application.PingTimer.Extensions;
using ApacBreachersRanked.Application.PingTimer.Models;
using ApacBreachersRanked.Application.PingTimer.Queries;
using Discord.Interactions;
using MediatR;

namespace ApacBreachersRanked.Modules
{
    public class PingTimerModule : BaseModule
    {
        public PingTimerModule(IMediator mediator) : base(mediator)
        {
        }

        [SlashCommand("pingtimers", "View the list of ping timers")]
        public async Task PingTimers()
        {
            IEnumerable<TimedPing> pingTimers = await _mediator.Send(new GetCurrentPingTimersQuery());
            await RespondAsync(embed: pingTimers.GetTimedPingsEmbed(), ephemeral: true);
        }
    }
}
