using System.Text.Json.Serialization;
using ApacBreachersRanked.Domain.Match.Enums;

namespace ApacBreachersRanked.Infrastructure.BreachersApi.Models;

public class BreachersGameData
{
    [JsonPropertyName("MapName")]
    public Map Map { get; set; }
    public int MatchMode { get; set; }
    public int MatchType { get; set; }
    public IEnumerable<BreachersPlayer> Players { get; set; }
    public IEnumerable<BreachersPlayer> PlayersLeft { get; set; }
}
