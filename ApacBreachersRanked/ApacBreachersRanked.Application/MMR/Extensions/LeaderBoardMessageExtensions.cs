using System.Text;
using ApacBreachersRanked.Application.Common.Extensions;
using ApacBreachersRanked.Application.MMR.Models;
using ApacBreachersRanked.Domain.Helpers;
using ApacBreachersRanked.Domain.Match.Enums;
using Discord;

namespace ApacBreachersRanked.Application.MMR.Extensions;

public static class LeaderBoardMessageExtensions
{
    public static Embed GetLeaderBoardEmbed(this IEnumerable<LeaderBoardPlayer> leaderBoardPlayers,
        MatchFormat matchFormat)
    {
        EmbedBuilder eb = new();
        eb.WithTitle($"APAC Breachers {matchFormat.GetFriendlyName()} Leaderboard");
        StringBuilder sb = new();
        var position = 1;
        foreach (var leaderBoardPlayer in leaderBoardPlayers)
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