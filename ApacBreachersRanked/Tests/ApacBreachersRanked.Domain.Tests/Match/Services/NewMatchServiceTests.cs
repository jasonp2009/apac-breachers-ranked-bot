using ApacBreachersRanked.Domain.Helpers;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.Match.Services;
using ApacBreachersRanked.Domain.MMR.Entities;
using ApacBreachersRanked.Domain.MMR.Services;
using ApacBreachersRanked.Domain.Tests.User;
using ApacBreachersRanked.Domain.User.Interfaces;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacBreachersRanked.Domain.Tests.Match.Services
{
    public class NewMatchServiceTests
    {
        private readonly IMMRService _mmrService;
        private readonly IMatchService _sut;

        public NewMatchServiceTests()
        {
            _mmrService = Substitute.For<IMMRService>();
            _sut = new MatchService(_mmrService);
        }

        [Fact]
        public async Task GivenAListOfDuplcatePlayerMMRs_WhenAllocatingTeams_AllocateTeamsWithNoMMRDifference()
        {
            // ARRANGE

            List<PlayerMMR> playerMMRs = new();

            for (int i = RandomExtensions.RandomNumber(2, 2); i >= 0; i--)
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
