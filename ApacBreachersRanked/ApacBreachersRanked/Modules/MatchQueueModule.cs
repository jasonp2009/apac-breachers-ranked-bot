using ApacBreachersRanked.Application.MatchQueue.Commands;
using Discord.Interactions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ApacBreachersRanked.Modules
{
    public class MatchQueueModule : BaseModule
    {
        private readonly ILogger<MatchQueueModule> _logger;
        public MatchQueueModule(IMediator mediator, ILogger<MatchQueueModule> logger)
            : base(mediator)
        {
            _logger = logger;
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
            } else if (response == "leave")
            {
                LeaveQueueCommand command = new LeaveQueueCommand()
                {
                    DiscordUserId = Context.User.Id
                };
                await _mediator.Send(command);
            } else
            {
                VoteToForceCommand command = new VoteToForceCommand()
                {
                    DiscordUserId = Context.User.Id
                };
                await _mediator.Send(command);
            }
            await DeferAsync();
        }

        [SlashCommand("forcematch", "Force a match to start")]
        [RequireRole("mod")]
        public async Task ForceMatchAsync()
        {
            try
            {
                await DeferAsync(ephemeral: true);
                await _mediator.Send(new ForceMatchCommand());
                await Context.Interaction.FollowupAsync("Match forced", ephemeral: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occur when trying to force match");
                await Context.Interaction.FollowupAsync(ex.Message, ephemeral: true);
            }
        }
    }
}
