using System.Text.Json.Serialization;

namespace ApacBreachersRanked.Infrastructure.Breachers.Models;

public class GameWeapon
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public WeaponType Type { get; set; }
    public int BotDamageDone { get; set; }
    public int BotHeadshotKills { get; set; }
    public int BotKills { get; set; }
    public int DamageDone { get; set; }
    public int FriendlyDamageDone { get; set; }
    public int HeadshotKills { get; set; }
    public int ShotsFired { get; set; }
    public int TotalHeadshots { get; set; }
    public int TotalKills { get; set; }
    public int TotalShotsHit { get; set; }
}
