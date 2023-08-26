using ApacBreachersRanked.Application.Common.Extensions;
using ApacBreachersRanked.Domain.MMR.Entities;
using Discord;
using System.Text;

namespace ApacBreachersRanked.Application.MMR.Extensions
{
    public static class LeaderBoardMessageExtensions
    {
        public static Embed GetLeaderBoardEmbed(this IEnumerable<PlayerMMR> playerMMRs)
        {
            EmbedBuilder eb = new();
            eb.WithTitle("APAC Breachers Leaderboard");
            StringBuilder sb = new();
            int position = 1;
            foreach (PlayerMMR playerMMR in playerMMRs)
            {
                sb.AppendLine($"{position}) {playerMMR.Rank.GetEmoji()} {playerMMR.GetUserMention()}: {playerMMR.MMR.ToString("0")}");
                position++;
            }
            eb.WithDescription(sb.ToString());
            return eb.Build();
        }
    }
}
