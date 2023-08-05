using ApacBreachersRanked.Application.Stats.Extensions;
using ApacBreachersRanked.Application.Stats.Models;
using ApacBreachersRanked.Application.Stats.Queries;
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
        public async Task Stats(SocketUser? user = null)
        {
            if (user == null) user = Context.User;
            await DeferAsync(ephemeral: true);
            BasicPlayerStats stats = await _mediator.Send(new GetBasicPlayerStatsQuery
            {
                DiscordUserId = user.Id
            });
            await Context.Interaction.FollowupAsync(embed: stats.GetBasicStatsEmbed(), ephemeral: true);
        }

        [SlashCommand("matches", "Get stats for your matches")]
        public async Task MatchStats(SocketUser? user = null)
        {
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
