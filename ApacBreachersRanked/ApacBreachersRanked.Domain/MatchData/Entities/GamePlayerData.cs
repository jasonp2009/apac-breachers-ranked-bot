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
    public IEnumerable<GamePlayerRoundData> Rounds { get; init; }
}