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
        foreach (var player in match.GameData.AllPlayers)
        {
            string playerCsv = "";
            playerCsv += $"{match.Id},";
            playerCsv += $"{match.TimeStampString},";
            playerCsv += $"{match.GameData.Map.ToString()},";
            playerCsv += $"{player.GameTime},";
            playerCsv += $"{player.Result.ToString()},";
            playerCsv += $"{player.Id},";
            playerCsv += $"{player.ClanTag},";
            playerCsv += $"{player.UserName},";
            playerCsv += $"{player.Rounds.Count(round => round.Team == round.TeamWon)},";
            playerCsv += $"{player.Rounds.Sum(round => round.Weapons.Sum(weapon => weapon.TotalKills) + round.Gadgets.Sum(gadget => gadget.Kills))},";
            playerCsv += $"{player.Rounds.Sum(round => round.Weapons.Sum(weapon => weapon.HeadshotKills))},";
            playerCsv += $"{player.Rounds.Sum(round => round.Weapons.Sum(weapon => weapon.TotalKills))},";
            playerCsv += $"{player.Rounds.Sum(round => round.Gadgets.Sum(gadget => gadget.Kills))},";
            playerCsv += $"{player.Rounds.Sum(round => round.Deaths)},";
            playerCsv += $"{player.Rounds.Sum(round => round.Assists)},";
            playerCsv += $"{player.Rounds.Sum(round => round.Weapons.Sum(weapon => weapon.DamageDone) + round.Gadgets.Sum(gadget => gadget.DamageDone))}\r\n";
            csv += playerCsv;
        }
        return csv;
    }

    private string GetHeadings()
    {
        string headings = "";
        headings += "GameId,";
        headings += "TimeStamp,";
        headings += "Map,";
        headings += "GameTime,";
        headings += "Result,";
        headings += "UserId,";
        headings += "ClanTag,";
        headings += "UserName,";
        headings += "RoundsWon,";
        headings += "Kills,";
        headings += "HeadshotKills,";
        headings += "WeaponKills,";
        headings += "GadgetKills,";
        headings += "Deaths,";
        headings += "Assists,";
        headings += "Damage\r\n";
        return headings;
    }
}
