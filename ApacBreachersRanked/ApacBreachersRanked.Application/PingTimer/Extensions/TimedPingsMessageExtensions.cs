using ApacBreachersRanked.Application.Common.Extensions;
using ApacBreachersRanked.Application.PingTimer.Models;
using Discord;
using System.Text;

namespace ApacBreachersRanked.Application.PingTimer.Extensions
{
    public static class TimedPingsMessageExtensions
    {
        public static Embed GetTimedPingsEmbed(this IEnumerable<TimedPing> timedPings)
        {
            EmbedBuilder eb = new();
            eb.WithTitle("Ping timers");

            StringBuilder sb = new();
            foreach (TimedPing timedPing in timedPings)
            {
                sb.Append($"<@&{timedPing.RoleId}> ");
                if (timedPing.IsTimedOut)
                {
                    sb.AppendLine($"is pingable {timedPing.NextPingUtc.AddSeconds(30).ToDiscordRelativeEpoch()}");
                } else
                {
                    sb.AppendLine($"is pingable");
                }
            }
            eb.WithDescription(sb.ToString());
            return eb.Build();
        }
    }
}
