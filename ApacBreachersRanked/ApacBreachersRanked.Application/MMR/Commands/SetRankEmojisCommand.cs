using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.Config;
using ApacBreachersRanked.Application.MMR.Extensions;
using ApacBreachersRanked.Domain.MMR.Enums;
using Discord;
using MediatR;
using Microsoft.Extensions.Options;

namespace ApacBreachersRanked.Application.MMR.Commands
{
    public class SetRankEmojisCommand : ICommand
    {
    }

    public class SetRankEmojisHandler : ICommandHandler<SetRankEmojisCommand>
    {
        private readonly IDiscordClient _discordClient;
        private readonly BreachersDiscordOptions _options;

        private readonly static string EmotePrefix = "ranked_";

        public SetRankEmojisHandler(IDiscordClient discordClient, IOptions<BreachersDiscordOptions> options)
        {
            _discordClient = discordClient;
            _options = options.Value;
        }
        public async Task<Unit> Handle(SetRankEmojisCommand request, CancellationToken cancellationToken)
        {
            IGuild guild = await _discordClient.GetGuildAsync(_options.GuildId);
            IReadOnlyCollection<GuildEmote> emotes = await guild.GetEmotesAsync();
            foreach (GuildEmote emote in emotes)
            {
                if (Enum.TryParse(typeof(Rank), emote.Name.Replace(EmotePrefix, string.Empty), true, out object rankObj) &&
                    rankObj is Rank rank)
                {
                    RankEmojiExtensions.SetRankEmoji(rank, emote.ToString());
                }
            }
            return Unit.Value;
        }
    }
}
