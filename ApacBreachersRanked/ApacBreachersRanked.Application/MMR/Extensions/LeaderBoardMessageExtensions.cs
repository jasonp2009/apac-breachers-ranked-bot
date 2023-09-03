using ApacBreachersRanked.Application.Common.Extensions;
using ApacBreachersRanked.Application.MMR.Models;
using Discord;
using System.Text;

namespace ApacBreachersRanked.Application.MMR.Extensions
{
    public static class LeaderBoardMessageExtensions
    {
        public static Embed GetLeaderBoardEmbed(this IEnumerable<LeaderBoardPlayer> leaderBoardPlayers)
        {
            EmbedBuilder eb = new();
            eb.WithTitle("APAC Breachers Leaderboard");
            StringBuilder sb = new();
            int position = 1;
            foreach (LeaderBoardPlayer leaderBoardPlayer in leaderBoardPlayers)
            {
                sb.Append($"{position}) ");
                sb.Append($"{leaderBoardPlayer.Rank.GetEmoji()} ");
                sb.Append($"{leaderBoardPlayer.GetUserMention()}: ");
                sb.Append($"{leaderBoardPlayer.MMR.ToString("0")} ");
                sb.Append($"[{leaderBoardPlayer.Matches.Won}/");
                sb.Append($"{leaderBoardPlayer.Matches.Drew}/");
                sb.Append($"{leaderBoardPlayer.Matches.Lost}/");
                sb.AppendLine($"{leaderBoardPlayer.Matches.Played}]");
                position++;
            }
            eb.WithDescription(sb.ToString());
            return eb.Build();
        }
    }
}
