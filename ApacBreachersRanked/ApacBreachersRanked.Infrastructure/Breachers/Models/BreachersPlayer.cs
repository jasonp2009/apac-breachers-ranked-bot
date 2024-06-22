using System.Text.Json.Serialization;

namespace ApacBreachersRanked.Infrastructure.Breachers.Models;

public class BreachersPlayer
{
    [JsonPropertyName("ApiId")]
    public string Id { get; set; }
    public string ClanTag { get; set; }
    [JsonPropertyName("GameMatchDataResult")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public GameResult Result { get; set; }
    public decimal GameTimeInSeconds { get; set; }
    public TimeSpan GameTime => TimeSpan.FromSeconds(Convert.ToDouble(GameTimeInSeconds));
    public bool Mvp { get; set; }
    public int PenaltyReason { get; set; }
    public IEnumerable<BreachersRound> Rounds { get; set; }
    public decimal TimeAfk { get; set; }
    [JsonPropertyName("Username")]
    public string UserName { get; set; }
}
