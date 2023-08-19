using ApacBreachersRanked.Application.Common.Extensions;
using ApacBreachersRanked.Application.Match.Models;
using ApacBreachersRanked.Domain.Helpers;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.Match.Enums;
using ApacBreachersRanked.Domain.MMR.Entities;
using Discord;
using System.Text;

namespace ApacBreachersRanked.Application.Match.Extensions
{
    public static class MatchMessageExtensions
    {
        public static string ConfirmedEmoji = "\u2705";
        public static string PendingConfirmationEmoji = "\u274C";
        public static string HostEmoji = "\uD83C\uDFE0";

        public static Color PendingColour = Color.Red;
        public static Color ConfirmedColour = Color.Green;

        public static Color HomeWinColour = new Color(48, 95, 167);
        public static Color AwayWinColour = new Color(192, 160, 51);
        public static Color DrawColour = Color.LightGrey;
        public static Embed GenerateMatchWelcomeEmbed(this MatchEntity match)
        {
            EmbedBuilder eb = new();
            eb.WithTitle("Welcome to the match");
            eb.WithDescription($"The match will begin once all players have confirmed{Environment.NewLine}" +
                               $"The match will auto-cancel {match.AutoCancelDateUtc.ToDiscordRelativeEpoch()} if all players have not confirmed");
            eb.AddTeamField($"Home ({match.HomeMMR.ToString("0")})", match.HomePlayers, true);
            eb.AddTeamField($"Away ({match.AwayMMR.ToString("0")})", match.AwayPlayers, true);
            if (match.AllPlayers.Any(x => !x.Confirmed))
            {
                eb.WithColor(PendingColour);
            } else
            {
                eb.WithColor(ConfirmedColour);
            }
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
            eb.AddTeamField($"Home ({match.HomeMMR.ToString("0")})", match.HomePlayers, false, true);
            eb.AddTeamField($"Away ({match.AwayMMR.ToString("0")})", match.AwayPlayers, false, true);
            eb.WithFooter("Once you have completed the match you can enter the score with /enterscore");
            eb.WithColor(Color.Green);
            return eb.Build();
        }

        public static Embed GeneratePendingMatchScoreEmbed(this PendingMatchScore matchScore)
        {
            EmbedBuilder eb = new();
            eb.WithTitle("Confirm score");
            eb.AddScoreFields(matchScore.Score);
            eb.AddTeamField("Home", matchScore.Players.Where(player => player.Side == MatchSide.Home));
            eb.AddTeamField("Away", matchScore.Players.Where(player => player.Side == MatchSide.Away));
            eb.WithColor(Color.Red);
            return eb.Build();
        }

        public static Embed GenerateMatchResultEmbed(this MatchEntity match, IEnumerable<MMRAdjustment> mmrAdjustments)
        {
            EmbedBuilder eb = new();
            eb.WithTitle($"Match #{match.MatchNumber}");
            eb.AddScoreFields(match.Score!);
            eb.AddTeamField($"Home ({match.HomeMMR.ToString("0")})", match.HomePlayers, withMMR: true, mmrAdjustments: mmrAdjustments);
            eb.AddTeamField($"Away ({match.AwayMMR.ToString("0")})", match.AwayPlayers, withMMR: true, mmrAdjustments: mmrAdjustments);
            switch(match.Score!.Outcome)
            {
                case ScoreOutcome.Home:
                    eb.WithColor(HomeWinColour);
                    break;
                case ScoreOutcome.Away:
                    eb.WithColor(AwayWinColour);
                    break;
                case ScoreOutcome.Draw:
                    eb.WithColor(DrawColour);
                    break;
            }
            return eb.Build();
        }

        private static void AddTeamField(this EmbedBuilder eb,
            string teamName, IEnumerable<MatchPlayer> players,
            bool withConfirmation = false, bool withHost = false,
            bool withMMR = true, IEnumerable<MMRAdjustment>? mmrAdjustments = null)
        {
            EmbedFieldBuilder efb = new();
            efb.WithName(teamName);
            efb.WithValue(players.Count() == 0 ? "No members" : string.Join(Environment.NewLine,
                players.Select(player => $"{player.GetPlayerMetion(withConfirmation, withHost, withMMR, mmrAdjustments?.FirstOrDefault(mmrAdj => mmrAdj.UserId.Equals(player.UserId))?.Adjustment)}")));
            efb.WithIsInline(true);
            eb.AddField(efb);
        }

        private static string GetPlayerMetion(this MatchPlayer player, bool withConfirmation = false, bool withHost = false, bool withMMR = true, decimal? mmrAdjustment = null)
        {
            StringBuilder sb = new();
            sb.Append(player.GetUserMention());
            if (withMMR)
            {
                sb.Append($" **{player.MMR.ToString("0")}**");
                if (mmrAdjustment != null)
                {
                    sb.Append(" (");
                    if (mmrAdjustment >= 0) sb.Append("+");
                    sb.Append($"{mmrAdjustment?.ToString("0.#")})");
                }
            }
            if (withConfirmation) sb.Append(" " + (player.Confirmed ? ConfirmedEmoji : PendingConfirmationEmoji));
            if (withHost && player.IsHost) sb.Append($" {HostEmoji}");
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
            sb.Append(player.GetUserMention());
            sb.Append(player.Confirmed ? ConfirmedEmoji : PendingConfirmationEmoji);
            return sb.ToString();
        }

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
