using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.Helpers;
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
        public static string HostEmoji = "\uD83C\uDFE0";
        public static Embed GenerateMatchWelcomeEmbed(this MatchEntity match)
        {
            EmbedBuilder eb = new();
            eb.WithTitle("Welcome to the match");
            eb.WithDescription("The match will begin once all players have confirmed");
            eb.AddTeamField("Home", match.HomePlayers, true);
            eb.AddTeamField("Away", match.AwayPlayers, true);
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
                string.Join(Environment.NewLine, match.HomePlayers.Select(homePlayer => $"    <@{homePlayer.GetPlayerMetion()}>"))
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
                string.Join(Environment.NewLine, match.AwayPlayers.Select(awayPlayer => $"    <@{awayPlayer.GetPlayerMetion()}>"))
            );
            return eb.Build();
        }

        public static Embed GenerateMatchConfirmedEmbed(this MatchEntity match)
        {
            EmbedBuilder eb = new();
            eb.WithTitle("The match is confirmed");
            eb.WithDescription(
                $"{match.HostPlayer?.Name} is host{Environment.NewLine}" +
                $"PW: {RandomExtensions.RandomNumber(10, 99)}{Environment.NewLine}" +
                $"Home has choice of map{Environment.NewLine}" +
                $"Away has choice of side{Environment.NewLine}");
            eb.AddTeamField("Home", match.HomePlayers, false, true);
            eb.AddTeamField("Away", match.AwayPlayers, false, true);
            return eb.Build();
        }

        private static void AddTeamField(this EmbedBuilder eb, string teamName, IEnumerable<MatchPlayer> players, bool withConfirmation = false, bool withHost = false)
        {
            EmbedFieldBuilder efb = new();
            efb.WithName(teamName);
            efb.WithValue(players.Count() == 0 ? "No members" : string.Join(Environment.NewLine, players.Select(player => $"{player.GetPlayerMetion(withConfirmation, withHost)}")));
            efb.WithIsInline(true);
            eb.AddField(efb);
        }

        private static string GetPlayerMetion(this MatchPlayer player, bool withConfirmation = false, bool withHost = false)
        {
            StringBuilder sb = new();
            sb.Append($"<@{player.UserId.GetDiscordId()}>");
            if (withConfirmation) sb.Append(player.Confirmed ? ConfirmedEmoji : PendingConfirmationEmoji);
            if (withHost && player.IsHost) sb.Append(HostEmoji);
            return sb.ToString();
        }
    }
}
