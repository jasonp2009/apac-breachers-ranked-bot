using ApacBreachersRanked.Application.Common.Extensions;
using ApacBreachersRanked.Application.Moderation.Models;
using Discord;
using System.Text;

namespace ApacBreachersRanked.Application.Moderation.Extensions
{
    public static class ActiveBansMessageExtensions
    {
        public static Embed GetActiveBanEmbed(this IEnumerable<UserBan> activeBans)
        {
            EmbedBuilder eb = new();
            eb.WithTitle("Active bans");
            foreach (UserBan activeBan in activeBans)
            {
                StringBuilder sb = new();
                sb.AppendLine($"{activeBan.GetUserMention()}'s ban will expire {activeBan.ExpiryUtc.ToDiscordRelativeEpoch()}");
                sb.AppendLine($"Ban reason: {activeBan.Reason}");
                eb.AddField(activeBan.Name, sb.ToString());
            }
            eb.WithColor(Color.Red);
            return eb.Build();
        }
    }
}
