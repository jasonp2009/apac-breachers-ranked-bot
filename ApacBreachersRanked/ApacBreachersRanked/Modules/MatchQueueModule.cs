using ApacBreachersRanked.Application.Common.Extensions;
using ApacBreachersRanked.Application.MatchQueue.Commands;
using ApacBreachersRanked.Application.MatchQueue.Exceptions;
using ApacBreachersRanked.Application.Moderation.Exceptions;
using ApacBreachersRanked.AutoCompleteHandlers;
using Discord.Interactions;
using MediatR;
using Microsoft.Extensions.Logging;
using TimeZoneConverter;

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
            catch (UserInMatchException)
            {
                await RespondAsync($"You are currently in an in-progress match. {Environment.NewLine}" +
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
                await RespondAsync(ephemeral: true, text: "This command is currently unavailable");
                return;
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

        [SlashCommand("playingat", "Schedule when you want to join queue")]
        public async Task PlayingAt(
            [Summary("When", "When you want to join the queue")]DateTime when,
            [Summary("Timezone"), Autocomplete(typeof(TimeZoneAutoCompleteHandler))]string timezone)
        {
            var tzi = TZConvert.GetTimeZoneInfo(timezone);
            var whenUtc = TimeZoneInfo.ConvertTimeToUtc(when, tzi);
            ScheduledJoinQueueCommand command = new()
            {
                DiscordUserId = Context.User.Id,
                JoinAtUtc = whenUtc
            };
            await _mediator.Send(command);
            await RespondAsync(ephemeral: true,
                text: $"You have been scheduled to join the queue at {whenUtc.ToDiscordFullEpoch()}");
        }

        [ComponentInteraction("scheduled-join-queue-*")]
        public async Task ScheduledJoinQueueAsync(string scheduledQueueId)
        {
            ScheduledJoinQueueByIdCommand command = new()
            {
                DiscordUserId = Context.User.Id,
                ScheduledQueueId = Guid.Parse(scheduledQueueId)
            };
            await _mediator.Send(command);
            await DeferAsync();
        }

        [ComponentInteraction("scheduled-leave-queue-*")]
        public async Task ScheduledLeaveQueueAsync(string scheduledQueueId)
        {
            ScheduledLeaveQueueCommand command = new()
            {
                DiscordUserId = Context.User.Id,
                ScheduledQueueId = Guid.Parse(scheduledQueueId)
            };
            await _mediator.Send(command);
            await DeferAsync();
        } 
    }
}
