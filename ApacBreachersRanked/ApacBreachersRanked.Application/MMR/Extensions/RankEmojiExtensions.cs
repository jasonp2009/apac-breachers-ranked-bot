using ApacBreachersRanked.Domain.MMR.Enums;

namespace ApacBreachersRanked.Application.MMR.Extensions
{
    public static class RankEmojiExtensions
    {
        public static string GetEmoji(this Rank? rank)
        {
            if (rank != null && rankEmojis.TryGetValue(rank.Value, out var emoji))
            {
                return emoji;
            }
            return "\u2754";
        }

        private static readonly Dictionary<Rank, string> rankEmojis = new();

        public static void SetRankEmoji(Rank rank, string emoji)
        {
            if (rankEmojis.ContainsKey(rank))
            {
                rankEmojis[rank] = emoji;
            } else
            {
                rankEmojis.Add(rank, emoji);
            }
        }
    }
}
