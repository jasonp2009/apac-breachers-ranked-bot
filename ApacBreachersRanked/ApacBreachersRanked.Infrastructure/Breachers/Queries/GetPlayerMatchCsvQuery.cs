using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Infrastructure.Breachers.Api;
using ApacBreachersRanked.Infrastructure.Breachers.Models;

namespace ApacBreachersRanked.Infrastructure.Breachers.Queries;

public class GetPlayerMatchCsvQuery : IQuery<string>
{
    public string BreachersPlayerId { get; set; }
    public int MatchNumber { get; set; } = 0;
}

public class GetPlayerMatchCsvHandler : IQueryHandler<GetPlayerMatchCsvQuery, string>
{
    private readonly BreachersApiClient _breachersApiClient;

    public GetPlayerMatchCsvHandler(BreachersApiClient breachersApiClient)
    {
        _breachersApiClient = breachersApiClient;
    }

    public async Task<string> Handle(GetPlayerMatchCsvQuery request, CancellationToken cancellationToken)
    {
        List<GetMatchResponse> matches = (await _breachersApiClient.GetMatchesByUserId(request.BreachersPlayerId, cancellationToken)).ToList();
        if (request.MatchNumber >= matches.Count)
        {
            throw new ArgumentException($"Can't find match number {request.MatchNumber}", nameof(request.MatchNumber));
        }

        GetMatchResponse match = matches[request.MatchNumber];
        string csv = GetHeadings();
        foreach (BreachersPlayer player in match.GameData.AllPlayers.OrderBy(player => player.Rounds.FirstOrDefault()?.Team))
        {
            csv += GetPlayerData(match, player);
        }
        return csv;
    }

    private string GetHeadings()
    {
        string headings = "";
        headings += "GameId,";
        headings += "UserId,";
        headings += "ClanTag,";
        headings += "UserName,";
        headings += "TimeStamp,";
        headings += "Map,";
        headings += "GameTime,";
        headings += "RoundsPlayed,";
        headings += "Result,";
        headings += "Score,";
        headings += "RoundsWon,";
        headings += "MVP,";
        headings += "Round MVPs,";
        headings += "Kills,";
        headings += "HeadshotKills,";
        headings += "WeaponKills,";
        headings += "GadgetKills,";
        headings += "Deaths,";
        headings += "Assists,";
        headings += "Weapon Damage,";
        headings += "Gadget Damage,";
        headings += "Damage,";
        headings += "Shots Fired,";
        headings += "Hits,";
        headings += "Headshot Hits\r\n";
        return headings;
    }

    private string GetPlayerData(GetMatchResponse match, BreachersPlayer player)
    {
        string playerCsv = "";
        playerCsv += $"{match.Id},";
        playerCsv += $"{player.Id},";
        playerCsv += $"{player.ClanTag},";
        playerCsv += $"{player.UserName},";
        playerCsv += $"{match.TimeStampString},";
        playerCsv += $"{match.GameData.Map.ToString()},";
        playerCsv += $"{player.GameTime},";
        playerCsv += $"{player.Rounds.Count()},";
        playerCsv += $"{player.Result.ToString()},";
        playerCsv += $"{player.Score},";
        playerCsv += $"{player.Rounds.Count(round => round.Team == round.TeamWon)},";
        playerCsv += $"{player.Mvp},";
        playerCsv += $"{player.Rounds.Count(round => round.Mvp)},";
        playerCsv += $"{player.Rounds.Sum(round => round.Weapons.Sum(weapon => weapon.TotalKills) + round.Gadgets.Sum(gadget => gadget.Kills))},";
        playerCsv += $"{player.Rounds.Sum(round => round.Weapons.Sum(weapon => weapon.HeadshotKills))},";
        playerCsv += $"{player.Rounds.Sum(round => round.Weapons.Sum(weapon => weapon.TotalKills))},";
        playerCsv += $"{player.Rounds.Sum(round => round.Gadgets.Sum(gadget => gadget.Kills))},";
        playerCsv += $"{player.Rounds.Sum(round => round.Deaths)},";
        playerCsv += $"{player.Rounds.Sum(round => round.Assists)},";
        playerCsv += $"{player.Rounds.Sum(round => round.Weapons.Sum(weapon => weapon.DamageDone))},";
        playerCsv += $"{player.Rounds.Sum(round => round.Gadgets.Sum(gadget => gadget.DamageDone))},";
        playerCsv += $"{player.Rounds.Sum(round => round.Weapons.Sum(weapon => weapon.DamageDone) + round.Gadgets.Sum(gadget => gadget.DamageDone))},";
        playerCsv += $"{player.Rounds.Sum(round => round.Weapons.Sum(weapon => weapon.ShotsFired))},";
        playerCsv += $"{player.Rounds.Sum(round => round.Weapons.Sum(weapon => weapon.TotalShotsHit))},";
        playerCsv += $"{player.Rounds.Sum(round => round.Weapons.Sum(weapon => weapon.TotalHeadshots))}\r\n";
        return playerCsv;
    }
}
