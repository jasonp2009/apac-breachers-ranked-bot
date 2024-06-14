namespace ApacBreachersRanked.Infrastructure.Config
{
    public class BreachersApiOptions
    {
        public static string Key = "BreachersApiOptions";
        public string BaseUrl { get; init; }
        public string ApiId { get; init; }
        public string Token { get; init; }
        public string Secret { get; init; }
    }
}
