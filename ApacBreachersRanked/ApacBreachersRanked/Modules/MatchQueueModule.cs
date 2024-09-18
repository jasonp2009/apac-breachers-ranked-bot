using System.Text;
using ApacBreachersRanked.Application.BreachersUsers.Exceptions;
using ApacBreachersRanked.Application.Common.Extensions;
using ApacBreachersRanked.Application.Common.Services;
using ApacBreachersRanked.Application.MatchQueue.Commands;
using ApacBreachersRanked.Application.MatchQueue.Exceptions;
using ApacBreachersRanked.Application.Moderation.Exceptions;
using ApacBreachersRanked.AutoCompleteHandlers;
using ApacBreachersRanked.Domain.Match.Enums;
using Discord.Interactions;
using MediatR;
using Microsoft.Extensions.Logging;
using TimeZoneConverter;

namespace ApacBreachersRanked.Modules
{
    public class MatchQueueModule : BaseModule
    {
        private readonly DateTimeParser _dateTimeParser;
        private readonly ILogger<MatchQueueModule> _logger;
        public MatchQueueModule(IMediator mediator,DateTimeParser dateTimeParser, ILogger<MatchQueueModule> logger)
            : base(mediator)
        {
            _dateTimeParser = dateTimeParser;
            _logger = logger;
        }

        [ComponentInteraction("join-queue-*-*")]
        public async Task JoinQueueAsync(int timeoutMins, MatchFormat matchFormat)
        {
            try
            {
                JoinQueueCommand command = new JoinQueueCommand()
                {
                    DiscordUserId = Context.User.Id,
                    TimeoutMins = timeoutMins,
                    MatchFormat = matchFormat
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
            catch (UserNotLinkedException)
            {
                await RespondAsync($"Please link your breachers account to your discord account. {Environment.NewLine}" +
                                        "http://apacbreachersranked.com",
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
            [Summary("When", "When you want to join the queue")]string when,
            [Summary("Timezone"), Autocomplete(typeof(TimeZoneAutoCompleteHandler))]string timezone)
        {
            List<DateTime> parsedResults;
            try
            {
                var tzi = TZConvert.GetTimeZoneInfo(timezone);
                parsedResults = await _dateTimeParser.Parse(when,tzi).ToListAsync();
            }
            catch (Exception)
            {
                await RespondAsync($"I am not quite sure what you mean by {when}", ephemeral: true);
                return;
            }
            switch (parsedResults.Count)
            {
                case 0:
                    await RespondAsync($"I am not quite sure what you mean by {when}", ephemeral: true);
                    return;
                case > 1:
                {
                    StringBuilder sb = new();
                    sb.AppendLine($"There are multiple possible values for the entered time {when}");
                    foreach (var parsedResult in parsedResults)
                    {
                        sb.AppendLine(parsedResult.ToDiscordFullEpoch());
                    }

                    await RespondAsync(sb.ToString(), ephemeral: true);
                    return;
                }
            }

            var whenUtc = parsedResults.Single();
            try
            {
                ScheduledJoinQueueCommand command = new()
                {
                    DiscordUserId = Context.User.Id,
                    JoinAtUtc = whenUtc
                };
                await _mediator.Send(command);
                await RespondAsync(ephemeral: true,
                    text: $"You have been scheduled to join the queue at {whenUtc.ToDiscordFullEpoch()}");
            }
            catch (UserNotLinkedException)
            {
                await RespondAsync($"Please link your breachers account to your discord account. {Environment.NewLine}" +
                                   "http://apacbreachersranked.com",
                    ephemeral: true);
            }
        }

        [ComponentInteraction("scheduled-join-queue-*")]
        public async Task ScheduledJoinQueueAsync(string scheduledQueueId)
        {
            try
            {
                ScheduledJoinQueueByIdCommand command = new()
                {
                    DiscordUserId = Context.User.Id,
                    ScheduledQueueId = Guid.Parse(scheduledQueueId)
                };
                await _mediator.Send(command);
                await DeferAsync();
            }
            catch (UserNotLinkedException)
            {
                await RespondAsync($"Please link your breachers account to your discord account. {Environment.NewLine}" +
                                   "http://apacbreachersranked.com",
                    ephemeral: true);
            }
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
