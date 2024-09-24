using ApacBreachersRanked.Application.Stats.Extensions;
using ApacBreachersRanked.Application.Stats.Models;
using ApacBreachersRanked.Application.Stats.Queries;
using ApacBreachersRanked.Domain.Match.Enums;
using Discord.Interactions;
using Discord.WebSocket;
using MediatR;

namespace ApacBreachersRanked.Modules
{
    [Group("stats", "Get stats")]
    public class StatModule : BaseModule
    {
        public StatModule(IMediator mediator) : base(mediator)
        {
        }

        [SlashCommand("basic", "Get basic stats")]
        public async Task Stats(
            [Summary("matchFormat", "The format for which you want to get stats for")]
            MatchFormat matchFormat,
            SocketUser? user = null)
        {
            await RespondAsync("This command is disabled", ephemeral: true);
            return;
            if (user == null) user = Context.User;
            await DeferAsync(ephemeral: true);
            BasicPlayerStats stats = await _mediator.Send(new GetBasicPlayerStatsQuery
            {
                DiscordUserId = user.Id,
                MatchFormat = matchFormat
            });
            await Context.Interaction.FollowupAsync(embed: stats.GetBasicStatsEmbed(), ephemeral: true);
        }

        [SlashCommand("matches", "Get stats for your matches")]
        public async Task MatchStats(SocketUser? user = null)
        {
            await RespondAsync("This command is disabled", ephemeral: true);
            return;
            if (user == null) user = Context.User;
            await DeferAsync(ephemeral: true);
            MatchesPlayerStats stats = await _mediator.Send(new GetMatchPlayerStatsQuery
            {
                DiscordUserId = user.Id
            });
            await Context.Interaction.FollowupAsync(embed: stats.GetMatchStatsEmbed(), ephemeral: true);
        }
    }
}
