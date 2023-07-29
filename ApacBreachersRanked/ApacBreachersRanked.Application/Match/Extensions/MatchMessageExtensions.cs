using ApacBreachersRanked.Application.Match.Models;
using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.Helpers;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.Match.Enums;
using Discord;
using System.Text;

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

        public static Embed GeneratePendingMatchScoreEmbed(this PendingMatchScore matchScore)
        {
            EmbedBuilder eb = new();
            eb.WithTitle("Confirm score");
            eb.AddScoreFields(matchScore.Score);
            eb.AddTeamField("Home", matchScore.Players.Where(player => player.Side == MatchSide.Home));
            eb.AddTeamField("Away", matchScore.Players.Where(player => player.Side == MatchSide.Away));
            return eb.Build();
        }

        public static Embed GenerateMatchResultEmbed(this MatchEntity match)
        {
            EmbedBuilder eb = new();
            eb.WithTitle($"Match #{match.MatchNumber}");
            eb.AddScoreFields(match.Score!);
            eb.AddTeamField("Home", match.HomePlayers);
            eb.AddTeamField("Away", match.HomePlayers);
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
            sb.Append(GetUserMention(player));
            if (withConfirmation) sb.Append(player.Confirmed ? ConfirmedEmoji : PendingConfirmationEmoji);
            if (withHost && player.IsHost) sb.Append(HostEmoji);
            return sb.ToString();
        }

        private static void AddTeamField(this EmbedBuilder eb, string teamName, IEnumerable<PendingMatchScorePlayer> players)
        {
            EmbedFieldBuilder efb = new();
            efb.WithName(teamName);
            efb.WithValue(players.Count() == 0 ? "No members" : string.Join(Environment.NewLine, players.Select(player => $"{player.GetPlayerMetion()}")));
            efb.WithIsInline(true);
            eb.AddField(efb);
        }

        private static string GetPlayerMetion(this PendingMatchScorePlayer player)
        {
            StringBuilder sb = new();
            sb.Append(GetUserMention(player));
            sb.Append(player.Confirmed ? ConfirmedEmoji : PendingConfirmationEmoji);
            return sb.ToString();
        }

        private static string GetUserMention(this Domain.User.Interfaces.IUser user)
            => $"<@{user.UserId.GetDiscordId()}>";

        private static void AddScoreFields(this EmbedBuilder eb, MatchScore score)
        {
            EmbedFieldBuilder efb = new();
            efb.WithName("Score");
            StringBuilder sb = new();
            sb.AppendLine(score.Outcome == ScoreOutcome.Draw ? "Match ended in a draw"
                                                             : $"{score.Outcome} won the match");
            sb.AppendLine($"Map score was {score.MapScore}");
            sb.AppendLine($"Round score was {score.RoundScore}");

            efb.WithValue(sb.ToString());
            eb.AddField(efb);

            eb.AddMapScoresField(score);
        }

        private static void AddMapScoresField(this EmbedBuilder eb, MatchScore score)
        {
            EmbedFieldBuilder efb = new();
            efb.WithName("Maps");
            StringBuilder sb = new();
            foreach (MapScore map in score.Maps)
            {
                sb.AppendLine($"{map.Map}: {map.ToString()} ({map.Outcome})");
            };

            efb.WithValue(sb.ToString());
            eb.AddField(efb);
        }
    }
}
