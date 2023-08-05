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

        [ComponentInteraction("match-queue-*")]
        public async Task JoinQueueAsync(string response)
        {
            if (response == "join")
            {
                JoinQueueCommand command = new JoinQueueCommand()
                {
                    DiscordUserId = Context.User.Id,
                    TimeoutMins = 60
                };
                await _mediator.Send(command);
            } else
            {
                LeaveQueueCommand command = new LeaveQueueCommand()
                {
                    DiscordUserId = Context.User.Id
                };
                await _mediator.Send(command);
            }
            await DeferAsync();
        }

        [SlashCommand("forcematch", "Force a match to start")]
        [RequireRole("Moderators")]
        public async Task ForceMatchAsync()
        {
            try
            {
                await DeferAsync(ephemeral: true);
                await _mediator.Send(new ForceMatchCommand());
                await DeleteOriginalResponseAsync();
            }
            catch (InvalidOperationException ex)
            {
                await Context.Interaction.FollowupAsync(ex.Message, ephemeral: true);
            }
        }
    }
}
