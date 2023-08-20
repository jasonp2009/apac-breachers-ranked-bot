using ApacBreachersRanked.Application.Common.Extensions;
using ApacBreachersRanked.Application.Match.Extensions;
using ApacBreachersRanked.Application.MatchVote.Enums;
using ApacBreachersRanked.Application.MatchVote.Models;
using ApacBreachersRanked.Domain.Match.Enums;
using Discord;
using System.Text;

namespace ApacBreachersRanked.Application.MatchVote.Extensions
{
    public static class MatchVoteMessageExtensions
    {
        public static Embed GetMapVoteEmbed(this MatchVoteModel matchVote)
        {
            EmbedBuilder eb = new();
            eb.WithTitle("Home is voting on map");
            StringBuilder sb = new();
            sb.AppendLine($"Voting will end {matchVote.HomeVoteEndUtc.ToDiscordRelativeEpoch()}");
            foreach (MatchVoteMap mapVote in matchVote.HomeVotes)
            {
                sb.Append(mapVote.GetUserMention());
                if (mapVote.Vote != null)
                {
                    sb.Append($" {mapVote.Vote.ToString()}");
                }
                sb.AppendLine();
            }
            eb.WithDescription(sb.ToString());
            if (matchVote.Status == MatchVoteStatus.VotingOnSide)
            {
                eb.WithColor(MatchMessageExtensions.ConfirmedColour);
            }
            else
            {
                eb.WithColor(MatchMessageExtensions.PendingColour);
            }
            return eb.Build();
        }

        public static MessageComponent GetMapVoteComponents(this MatchVoteModel matchVote)
        {
            ComponentBuilder cb = new();
            foreach (Map map in Enum.GetValues(typeof(Map)).Cast<Map>())
            {
                cb.WithButton(map.ToString(), $"mapvote-{matchVote.Match.MatchNumber}-{map}");
            }
            return cb.Build();
        }

        public static Embed GetSideVoteEmbed(this MatchVoteModel matchVote)
        {
            EmbedBuilder eb = new();
            eb.WithTitle("Away is voting on side");
            eb.WithFooter($"Home voted for {matchVote.VotedMap}");
            StringBuilder sb = new();
            sb.AppendLine($"Voting will end {matchVote.AwayVoteEndUtc?.ToDiscordRelativeEpoch()}");
            foreach (MatchVoteSide sideVote in matchVote.AwayVotes)
            {
                sb.Append(sideVote.GetUserMention());
                if (sideVote.Vote != null)
                {
                    sb.Append($" {sideVote.Vote.ToString()}");
                }
                sb.AppendLine();
            }
            eb.WithDescription(sb.ToString());
            if (matchVote.Status == MatchVoteStatus.VotingComplete)
            {
                eb.WithColor(MatchMessageExtensions.ConfirmedColour);
            } else
            {
                eb.WithColor(MatchMessageExtensions.PendingColour);
            }
            return eb.Build();
        }

        public static MessageComponent GetSideVoteComponents(this MatchVoteModel matchVote)
        {
            ComponentBuilder cb = new();
            foreach (GameSide side in Enum.GetValues(typeof(GameSide)).Cast<GameSide>())
            {
                cb.WithButton(side.ToString(), $"sidevote-{matchVote.Match.MatchNumber}-{side}");
            }
            return cb.Build();
        }
    }
}
