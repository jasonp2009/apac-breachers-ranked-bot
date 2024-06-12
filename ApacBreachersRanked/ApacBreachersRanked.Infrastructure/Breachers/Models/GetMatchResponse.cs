using System.Text.Json.Serialization;

namespace ApacBreachersRanked.Infrastructure.Breachers.Models;

public class GetMatchResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("players")]
    public int Players { get; set; }
    [JsonPropertyName("timestamp")]
    public DateTime TimeStamp { get; set; }
    [JsonPropertyName("clan_game")]
    public object ClanGame { get; set; }
    [JsonPropertyName("game_data")]
    public BreachersGameData GameData { get; set; }
}
