namespace ApacBreachersRanked.Infrastructure.BreachersApi.Models;

public class GameWeapon
{
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
