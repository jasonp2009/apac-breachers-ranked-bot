using Newtonsoft.Json;

namespace ApacBreachersRanked.Infrastructure.SQS.Extensions
{
    internal static class MessageSerializer
    {
        public static string Serialize<T>(T message)
            => JsonConvert.SerializeObject(message, JsonSerializerSettings);

        public static T? Deserialize<T>(string message)
            => JsonConvert.DeserializeObject<T>(message, JsonSerializerSettings);

        private static JsonSerializerSettings JsonSerializerSettings => new()
        {
            TypeNameHandling = TypeNameHandling.All
        };
    }
}
