using ApacBreachersRanked.Application.Common.Extensions;
using ApacBreachersRanked.Application.MatchQueue.Commands;
using ApacBreachersRanked.Application.MatchQueue.Exceptions;
using ApacBreachersRanked.Application.Moderation.Exceptions;
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

        [ComponentInteraction("join-queue-*")]
        public async Task JoinQueueAsync(int timeoutMins)
        {
            try
            {
                JoinQueueCommand command = new JoinQueueCommand()
                {
                    DiscordUserId = Context.User.Id,
                    TimeoutMins = timeoutMins
                };
                await _mediator.Send(command);
            }
            catch (UserBannedException ex)
            {
                await RespondAsync($"You cannot join the queue due to an active ban.{Environment.NewLine}" +
                                   $"Your ban will expire {ex.ExpiryUtc.ToDiscordRelativeEpoch()}{Environment.NewLine}" +
                                   $"Ban reason: {ex.Reason}",
                                   ephemeral: true);
                return;
            }
            catch (UserInMatchException ex)
            {
                await RespondAsync($"You are currently in an in-progress match #{ex.MatchNumber}. {Environment.NewLine}" +
                                    "Please complete the match and confirm the score before joining the queue.",
                                    ephemeral: true);
                return;
            }
            await DeferAsync();
        }

        [ComponentInteraction("leave-queue")]
        public async Task LeaveQueueAsync()
        {
            LeaveQueueCommand command = new LeaveQueueCommand()
            {
                DiscordUserId = Context.User.Id
            };
            await _mediator.Send(command);
            await DeferAsync();
        }

        [ComponentInteraction("vote-force-match")]
        public async Task VoteForceMatchAsync()
        {
            VoteToForceCommand command = new VoteToForceCommand()
            {
                DiscordUserId = Context.User.Id
            };
            await _mediator.Send(command);
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
