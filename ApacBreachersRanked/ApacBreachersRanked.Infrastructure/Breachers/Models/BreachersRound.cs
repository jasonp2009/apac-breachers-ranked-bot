using System.Text.Json.Serialization;

namespace ApacBreachersRanked.Infrastructure.Breachers.Models;

public class BreachersRound
{
    public bool Ace { get; set; }
    public int Assists { get; set; }
    public int BotAssists { get; set; }
    public int Deaths { get; set; }
    public bool FirstBlood { get; set; }
    public bool Mvp { get; set; }
    public int RoundNumber { get; set; }
    public decimal RoundTime { get; set; }
    public int Score { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public BreachersSide Team { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public BreachersSide TeamWon { get; set; }
    public IEnumerable<GameGadget> Gadgets { get; set; }
    public IEnumerable<GameWeapon> Weapons { get; set; }
}
