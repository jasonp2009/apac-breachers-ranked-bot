using ApacBreachersRanked.Application.Stats.Models;
using Discord;
using System.Text;

namespace ApacBreachersRanked.Application.Stats.Extensions
{
    public static class BasicStatsMessageExtensions
    {
        public static Embed GetBasicStatsEmbed(this BasicPlayerStats stats)
        {
            EmbedBuilder eb = new();
            eb.WithTitle($"{stats.User.Name}'s stats");
            StringBuilder sb = new();
            sb.AppendLine($"MMR: {stats.MMR.ToString("0")}");
            sb.AppendLine($"Matches Played: {stats.Match.Played}");
            sb.AppendLine($"Matches Won: {stats.Match.Won}");
            sb.AppendLine($"Matches Drew: {stats.Match.Drew}");
            sb.AppendLine($"Matches Lost: {stats.Match.Lost}");
            eb.WithDescription(sb.ToString());
            return eb.Build();
        }
    }
}
