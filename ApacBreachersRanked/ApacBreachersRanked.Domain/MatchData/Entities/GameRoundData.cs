using ApacBreachersRanked.Domain.Match.Enums;
using ApacBreachersRanked.Domain.User.Interfaces;

namespace ApacBreachersRanked.Domain.MatchData.Entities;

public class GameRoundData
{
    public int RoundNumber => Players.FirstOrDefault()?.RoundNumber ?? 0;
    public MatchSide? Winner => Players.FirstOrDefault(player => player.Won)?.Side;
    public IUser HomeMvp => Players.FirstOrDefault(player => player.Side == MatchSide.Home && player.Mvp);
    public IUser AwayMvp => Players.FirstOrDefault(player => player.Side == MatchSide.Away && player.Mvp);
    public int Damage => Players?.Sum(player => player.Damage) ?? 0;
    public int Kills => Players?.Sum(player => player.Kills) ?? 0;
    public int Assists => Players?.Sum(player => player.Assists) ?? 0;
    public int Deaths => Players?.Count(player => player.Died) ?? 0;

    public IEnumerable<WeaponData> Weapons => Players
        .SelectMany(player => player.Weapons)
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
    

    public IEnumerable<GadgetData> Gadgets => Players
        .SelectMany(player => player.Gadgets)
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
    public IEnumerable<GamePlayerRoundData> Players { get; init; }
}
