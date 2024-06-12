using System.Text.Json.Serialization;

namespace ApacBreachersRanked.Infrastructure.Breachers.Models;

public class GameGadget
{
    [JsonPropertyName("Name")]
    public GadgetType Type { get; set; }
    public int BotDamageDone { get; set; }
    public int BotKills { get; set; }
    public int DamageDone { get; set; }
    public int Destroyed { get; set; }
    public int EnemyTriggered { get; set; }
    public int FriendlyDamageDone { get; set; }
    public int Kills { get; set; }
    public int TeamHealed { get; set; }
    public int Triggered { get; set; }
    public int Used { get; set; }
}
