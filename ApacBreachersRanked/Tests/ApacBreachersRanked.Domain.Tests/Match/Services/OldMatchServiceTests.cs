using ApacBreachersRanked.Domain.Helpers;
using ApacBreachersRanked.Domain.MMR.Entities;
using ApacBreachersRanked.Domain.Tests.User;
using FluentAssertions;
using ApacBreachersRanked.Domain.Match.Services;
using ApacBreachersRanked.Domain.MMR.Services;
using NSubstitute;
using ApacBreachersRanked.Domain.User.Interfaces;
using ApacBreachersRanked.Domain.Match.Entities;

namespace ApacBreachersRanked.Tests.Match.Services
{
    public class OldMatchServiceTests
    {
        private readonly IMMRService _mmrService;
        private readonly IMatchService _sut;

        public OldMatchServiceTests()
        {
            _mmrService = Substitute.For<IMMRService>();
            _sut = new OldMatchService(_mmrService);
        }

        [Fact]
        public async Task GivenAListOfDuplcatePlayerMMRs_WhenAllocatingTeams_AllocateTeamsWithNoMMRDifference()
        {
            // ARRANGE
            
            List<PlayerMMR> playerMMRs = new();

            for(int i = RandomExtensions.RandomNumber(2,2); i >= 0; i--)
            {
                decimal mmr = RandomExtensions.RandomNumber(100, 2000);
                playerMMRs.Add(new PlayerMMR(new TestUser(), mmr));
                playerMMRs.Add(new PlayerMMR(new TestUser(), mmr));
            }

            _mmrService.GetPlayerMMRsAsync(Arg.Any<IEnumerable<IUser>>()).Returns(playerMMRs);

            // ACT
            MatchEntity result = await _sut.CreateMatchFromQueueAsync(new(), default);

            // ASSERT
            decimal totalHomeMMR = playerMMRs.Where(x => result.HomePlayers.Any(player => player.UserId.Equals(x.UserId))).Sum(x => x.MMR);
            decimal totalAwayMMR = playerMMRs.Where(x => result.AwayPlayers.Any(player => player.UserId.Equals(x.UserId))).Sum(x => x.MMR);

            totalHomeMMR.Should().Be(totalAwayMMR);
            
        }

        [Fact]
        public async Task TestBruteForceAgainstGreedy()
        {
            for(int rep = 0; rep < 1000; rep++)
            {
                // ARRANGE
                List<PlayerMMR> playerMMRs = new();

                for (int i = RandomExtensions.RandomNumber(2, 100); i > 0; i--)
                {
                    decimal mmr = RandomExtensions.RandomNumber(800, 1200);
                    playerMMRs.Add(new PlayerMMR(new TestUser(), mmr));
                }



                decimal reasonableMaxDiff = Math.Abs(playerMMRs.OrderByDescending(x => x.MMR).Where((x, i) => i % 2 == 0).Average(x => x.MMR) - playerMMRs.OrderByDescending(x => x.MMR).Where((x, i) => i % 2 == 1).Average(x => x.MMR))*10;

                _mmrService.GetPlayerMMRsAsync(Arg.Any<IEnumerable<IUser>>()).Returns(playerMMRs);

                // ACT
                MatchEntity greedyResult = await _sut.CreateMatchFromQueueAsync(new(), default);

                decimal totalHomeMMR = playerMMRs.Where(x => greedyResult.HomePlayers.Any(player => player.UserId.Equals(x.UserId))).Sum(x => x.MMR);
                decimal totalAwayMMR = playerMMRs.Where(x => greedyResult.AwayPlayers.Any(player => player.UserId.Equals(x.UserId))).Sum(x => x.MMR);

                decimal greedyDiff = Math.Abs(totalHomeMMR - totalAwayMMR);

                // ASSERT
                greedyResult.HomePlayers.Should().HaveCountGreaterThanOrEqualTo(Math.Min(5, (int)(playerMMRs.Count / 2)));
                greedyResult.AwayPlayers.Should().HaveCountGreaterThanOrEqualTo(Math.Min(5, (int)(playerMMRs.Count / 2)));
                greedyResult.HomePlayers.Should().HaveCountLessThanOrEqualTo(5);
                greedyResult.AwayPlayers.Should().HaveCountLessThanOrEqualTo(5);

                //greedyDiff.Should().BeLessThanOrEqualTo(Math.Max(reasonableMaxDiff, 10), $"Run {rep}");
            }
        }

        [Fact]
        public async Task GivenAListOfPlayers_WhenAssigningTeams_EnsureTop2PlayersAreDivided()
        {
            TestUser bestPlayer = new TestUser();
            TestUser secondBestPlayer = new TestUser();

            List<PlayerMMR> players = new()
                {
                    new PlayerMMR(bestPlayer, 125),
                    new PlayerMMR(secondBestPlayer, 120),
                    new PlayerMMR(new TestUser(), 110),
                    new PlayerMMR(new TestUser(), 100),
                    new PlayerMMR(new TestUser(), 90),
                    new PlayerMMR(new TestUser(), 55)
                };


            _mmrService.GetPlayerMMRsAsync(Arg.Any<IEnumerable<IUser>>()).Returns(players);

            MatchEntity result = await _sut.CreateMatchFromQueueAsync(new(), default);

            if (result.HomePlayers.Any(x => x.Equals(bestPlayer)))
            {
                Assert.Contains(result.AwayPlayers, x => x.UserId.Equals(secondBestPlayer.UserId));
            }
            {
                Assert.Contains(result.HomePlayers, x => x.UserId.Equals(secondBestPlayer.UserId));
            }
        }
    }
}
