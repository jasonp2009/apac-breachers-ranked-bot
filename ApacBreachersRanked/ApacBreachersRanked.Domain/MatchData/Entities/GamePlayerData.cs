using ApacBreachersRanked.Domain.Match.Enums;
using ApacBreachersRanked.Domain.MMR.Enums;
using ApacBreachersRanked.Domain.User.Interfaces;

namespace ApacBreachersRanked.Domain.MatchData.Entities;

public class GamePlayerData : IUser
{
    public IUserId UserId { get; init; }
    public string? Name { get; init; }
    public decimal Mmr { get; init; }
    public Rank? Rank { get; init; }
    public MatchSide Side { get; init; }
    public TimeSpan GameTime { get; init; }
    public int Kills => Rounds.Sum(round => round.Kills);
    public int Assists => Rounds.Sum(round => round.Assists);
    public int Deaths => Rounds.Count(round => round.Died);
    public int Damage => Rounds.Sum(round => round.Damage);
    public int RoundMvps => Rounds.Count(round => round.Mvp);
    public bool Mvp { get; init; }
    public int Aces => Rounds.Count(round => round.Ace);
    public IEnumerable<WeaponData> Weapons => Rounds
        .SelectMany(round => round.Weapons)
        .GroupBy(
            weapon => weapon.Type,
            weapon => weapon,
            (key, group) =>
            {
                List<WeaponData> groupList = group.ToList();
                return new WeaponData
                {
                    Type = key,
                    Damage = groupList.Sum(x => x.Damage),
                    FriendlyDamage = groupList.Sum(x => x.FriendlyDamage),
                    HeadshotKills = groupList.Sum(x => x.HeadshotKills),
                    Headshots = groupList.Sum(x => x.Headshots),
                    Hits = groupList.Sum(x => x.Hits),
                    Kills = groupList.Sum(x => x.Kills),
                    ShotsFired = groupList.Sum(x => x.ShotsFired)
                };
            });
    

    public IEnumerable<GadgetData> Gadgets => Rounds
        .SelectMany(round => round.Gadgets)
        .GroupBy(
            gadget => gadget.Type,
            gadget => gadget,
            (key, group) =>
            {
                List<GadgetData> groupList = group.ToList();
                return new GadgetData
                {
                    Type = key,
                    Damage = groupList.Sum(x => x.Damage),
                    Destroyed = groupList.Sum(x => x.Destroyed),
                    EnemyTriggered = groupList.Sum(x => x.EnemyTriggered),
                    FriendlyDamage = groupList.Sum(x => x.FriendlyDamage),
                    Healed = groupList.Sum(x => x.Healed),
                    Kills = groupList.Sum(x => x.Kills),
                    Triggered = groupList.Sum(x => x.Triggered),
                    Used = groupList.Sum(x => x.Used)
                };
            });
    public IEnumerable<GamePlayerRoundData> Rounds { get; init; }
}