namespace ApacBreachersRanked.Application.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static int ToEpoch(this DateTime utcDateTime)
        {
            TimeSpan t = utcDateTime - new DateTime(1970, 1, 1);
            return (int)t.TotalSeconds;
        }

        public static string ToDiscordRelativeEpoch(this DateTime utcDateTime)
            => $"<t:{utcDateTime.ToEpoch()}:R>";
    }
}
