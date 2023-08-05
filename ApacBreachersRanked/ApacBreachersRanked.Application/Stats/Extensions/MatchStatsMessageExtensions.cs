using ApacBreachersRanked.Application.Stats.Models;
using Discord;

namespace ApacBreachersRanked.Application.Stats.Extensions
{
    public static class MatchStatsMessageExtensions
    {
        public static Embed GetMatchStatsEmbed(this MatchesPlayerStats stats)
        {
            EmbedBuilder eb = new();
            eb.WithTitle($"{stats.User.Name}'s match stats");
            eb.WithDescription($"MMR: {stats.MMR.ToString("0.##")}");
            foreach (MatchPlayerStats match in stats.Matches)
            {
                eb.AddField($"Match #{match.MatchNumber}",
                    $"{match.Outcome} ({match.RoundsWon}-{match.RoundsLost}){Environment.NewLine}" +
                    $"MMR Adjustment: {match.MMRAdjustment.ToString("0.##")}");
            }
            return eb.Build();
        }
    }
}
