using System.Text.Json.Serialization;

namespace ApacBreachersRanked.Infrastructure.Breachers.Models;

public class GetMatchResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("players")]
    public int Players { get; set; }
    [JsonPropertyName("timestamp")]
    public string TimeStampString { get; set; }
    [JsonIgnore]
    public DateTime TimeStamp => DateTime.Parse(TimeStampString);
    [JsonPropertyName("game_data")]
    public BreachersGameData GameData { get; set; }
}
