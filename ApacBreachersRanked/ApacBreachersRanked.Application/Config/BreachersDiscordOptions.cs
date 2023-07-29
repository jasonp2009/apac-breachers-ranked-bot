namespace ApacBreachersRanked.Application.Config
{
    public class BreachersDiscordOptions
    {
        public static string Key = nameof(BreachersDiscordOptions);
        public ulong GuildId { get; init; }
        public ulong ReadyUpChannelId { get; init; }
        public ulong MatchChannelId { get; init; }
        public ulong MatchResultsChannelId { get; init; }
    }
}
