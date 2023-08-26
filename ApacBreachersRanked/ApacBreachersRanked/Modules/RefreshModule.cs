using ApacBreachersRanked.Application.MMR.Commands;
using Discord.Interactions;
using MediatR;

namespace ApacBreachersRanked.Modules
{
    [RequireOwner]
    [Group("refresh", "Dev commands to force updates to discord")]
    public class RefreshModule : BaseModule
    {
        public RefreshModule(IMediator mediator) : base(mediator)
        {
        }

        [SlashCommand("leaderboard", "Force a refresh of the leaderboard message")]
        public Task LeaderBoard()
            => _mediator.Send(new RefreshLeaderboardCommand(), CancellationToken.None);

        [SlashCommand("rank_emojis", "Force an update of the rank emojis if they were modified")]
        public Task RankEmojis()
            => _mediator.Send(new SetRankEmojisCommand(), CancellationToken.None);
    }
}
