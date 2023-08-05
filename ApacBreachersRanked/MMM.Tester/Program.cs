// See https://aka.ms/new-console-template for more information


using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.Match.Enums;
using ApacBreachersRanked.Domain.MMR.Entities;
using ApacBreachersRanked.Domain.MMR.Services;
using MMM.Tester;

List<decimal> mapWeightings = new() { 0.55M, 0.5M };

List<(int, int)> posibleScores = new()
{
    ( 7,0 ),
    ( 7,1 ),
    ( 7,2 ),
    ( 7,3 ),
    ( 7,4 ),
    ( 7,5 ),
    ( 6,6 ),
    ( 5,7 ),
    ( 4,7 ),
    ( 3,7 ),
    ( 2,7 ),
    ( 1,7 ),
    ( 0,7 )
};

List<PlayerMMR> homePlayers = new List<PlayerMMR>();
List<PlayerMMR> awayPlayers = new List<PlayerMMR>();

for (int i = 0; i < 1; i++)
{
    homePlayers.Add(new PlayerMMR(new TestUser()));
}

for (int i = 0; i < 1; i++)
{
    awayPlayers.Add(new PlayerMMR(new TestUser()));
}

Dictionary<decimal,Dictionary<(int, int), decimal>> allOutcomes = new();


foreach (decimal mapWeighting in mapWeightings)
{
    Dictionary<(int, int), decimal> outcomes = new();

    foreach((int home, int away) in posibleScores)
    {
        IMMRAdjustmentService mmrAdjustmentService = new MMRAdjustmentServiceTester(new()
        {
            KFactor = 24,
            MapWeighting = mapWeighting,
            RoundWeighting = 1 - mapWeighting
        });

        MatchScore score = new();
        score.Maps.Add(new MapScore(Map.Hideout, home, away));

        decimal outcome = mmrAdjustmentService.CalculateTeamMMRAdjustment(score, homePlayers, awayPlayers);

        outcomes.Add((home, away), outcome);
    }
    allOutcomes.Add(mapWeighting, outcomes);
}

Console.WriteLine(allOutcomes);

// string.Join("\n", allOutcomes[0.75M].Values.Select(x => x.ToString("0.##")))