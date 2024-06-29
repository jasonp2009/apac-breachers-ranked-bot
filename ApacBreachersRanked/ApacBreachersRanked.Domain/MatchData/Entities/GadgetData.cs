using ApacBreachersRanked.Domain.MatchData.Enums;

namespace ApacBreachersRanked.Domain.MatchData.Entities;

public class GadgetData
{
    public GadgetType Type { get; init; }
    public int Used { get; init; }
    public int Triggered { get; init; }
    public int EnemyTriggered { get; init; }
    public int Kills { get; init; }
    public int Damage { get; init; }
    public int FriendlyDamage { get; init; }
    public int Destroyed { get; init; }
    public int Healed { get; init; }
}