using ApacBreachersRanked.Domain.MatchData.Enums;

namespace ApacBreachersRanked.Domain.MatchData.Entities;

public class WeaponData
{
    public WeaponType Type { get; init; }
    public int Kills { get; init; }
    public int HeadshotKills { get; init; }
    public int ShotsFired { get; init; }
    public int Hits { get; init; }
    public int Headshots { get; init; }
    public int Damage { get; init; }
    public int FriendlyDamage { get; init; }
}