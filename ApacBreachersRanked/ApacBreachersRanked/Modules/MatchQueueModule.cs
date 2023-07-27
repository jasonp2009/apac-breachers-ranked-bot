using ApacBreachersRanked.Application.MatchQueue.Commands;
using Discord.Interactions;
using MediatR;

namespace ApacBreachersRanked.Modules
{
    public class MatchQueueModule : BaseModule
    {

        public MatchQueueModule(IMediator mediator)
            : base(mediator)
        {
        }

        [SlashCommand("joinqueue", "Join the ranked queue")]
        public async Task JoinQueueAsync(int timeoutMins = 60)
        {
            AddUserToQueueCommand command = new AddUserToQueueCommand()
            {
                DiscordUserId = Context.User.Id,
                TimeoutMins = timeoutMins
            };
            await _mediator.Send(command);
        }

        [SlashCommand("forcematch", "Force a match to start")]
        [RequireRole("Developers")]
        public async Task ForceMatchAsync()
        {
            await _mediator.Send(new ForceMatchCommand());
        }
    }
}
