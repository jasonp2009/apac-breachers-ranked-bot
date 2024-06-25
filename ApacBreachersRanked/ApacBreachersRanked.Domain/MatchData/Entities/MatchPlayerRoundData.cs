namespace ApacBreachersRanked.Domain.MatchData.Models;

public class MatchPlayerRoundData
{
    public int RoundNumber { get; init; }
    public int Kills => Weapons.Sum(weapon => weapon.Kills) + Gadgets.Sum(gadget => gadget.Kills);
    public int Assists { get; init; }
    public bool Died { get; init; }
    public int Damage => Weapons.Sum(weapon => weapon.Damage) + Gadgets.Sum(gadget => gadget.Damage);
    public bool FirstBlood { get; init; }
    public bool Mvp { get; init; }
    public bool Ace { get; init; }
    public IEnumerable<WeaponData> Weapons { get; init; }
    public IEnumerable<GadgetData> Gadgets { get; init; }
}