using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.Match.Entities;
using Discord;
using System.Text;
using System.Text.RegularExpressions;

namespace ApacBreachersRanked.Application.Match.Extensions
{
    public static class MatchMessageExtensions
    {
        public static string ConfirmedEmoji = "\u2705";
        public static string PendingConfirmationEmoji = "\u274C";
        public static Embed GenerateMatchEmbed(this MatchEntity match)
        {
            EmbedBuilder eb = new();
            eb.WithTitle("Welcome to the match");
            eb.AddTeamField("Home", match.HomePlayers);
            eb.AddTeamField("Away", match.AwayPlayers);
            eb.WithFooter("Please type /confirm to confirm you are ready to play the match");
            return eb.Build();
        }

        public static Embed GenerateHomeEmbed(this MatchEntity match)
        {
            EmbedBuilder eb = new();
            eb.WithTitle("Welcome to the match Home team");
            eb.WithDescription(
                $"This is a thread for just your team{Environment.NewLine}" +
                $"You team is:{Environment.NewLine}" +
                string.Join(Environment.NewLine, match.HomePlayers.Select(homePlayer => $"    <@{homePlayer.GetMentionWithReadyStatus()}>"))
            );
            return eb.Build();
        }

        public static Embed GenerateAwayEmbed(this MatchEntity match)
        {
            EmbedBuilder eb = new();
            eb.WithTitle("Welcome to the match Away team");
            eb.WithDescription(
                $"This is a thread for just your team{Environment.NewLine}" +
                $"You team is:{Environment.NewLine}" +
                string.Join(Environment.NewLine, match.AwayPlayers.Select(awayPlayer => $"    <@{awayPlayer.GetMentionWithReadyStatus()}>"))
            );
            return eb.Build();
        }

        private static void AddTeamField(this EmbedBuilder eb, string teamName, IEnumerable<MatchPlayer> players)
        {
            EmbedFieldBuilder efb = new();
            efb.WithName(teamName);
            efb.WithValue(players.Count() == 0 ? "No members" : string.Join(Environment.NewLine, players.Select(player => $"{player.GetMentionWithReadyStatus()}")));

            eb.AddField(efb);
        }

        private static string GetMentionWithReadyStatus(this MatchPlayer player)
        {
            return $"<@{player.UserId.GetDiscordId()}> " + (player.Confirmed ? ConfirmedEmoji : PendingConfirmationEmoji);
        }
    }
}
