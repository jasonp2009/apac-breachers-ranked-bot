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
using ApacBreachersRanked.Domain.Match.Enums;

namespace ApacBreachersRanked.Domain.Tests.Match.Services
{
    public class NewMatchServiceTests
    {
        private readonly IMmrService _mmrService;
        private readonly IMatchService _sut;

        public NewMatchServiceTests()
        {
            _mmrService = Substitute.For<IMmrService>();
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
                playerMMRs.Add(new PlayerMMR(new TestUser(), MatchFormat.Ranked, mmr));
                playerMMRs.Add(new PlayerMMR(new TestUser(), MatchFormat.Ranked, mmr));
            }

            _mmrService.GetPlayerMmRsAsync(Arg.Any<IEnumerable<IUser>>(), MatchFormat.Ranked).Returns(playerMMRs);

            // ACT
            MatchEntity result = await _sut.CreateMatchFromQueueAsync(new(MatchFormat.Ranked), default);

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
                    new PlayerMMR(bestPlayer, MatchFormat.Ranked, 125),
                    new PlayerMMR(secondBestPlayer, MatchFormat.Ranked, 120),
                    new PlayerMMR(new TestUser(), MatchFormat.Ranked, 110),
                    new PlayerMMR(new TestUser(), MatchFormat.Ranked, 100),
                    new PlayerMMR(new TestUser(), MatchFormat.Ranked, 90),
                    new PlayerMMR(new TestUser(), MatchFormat.Ranked, 55)
                };


            _mmrService.GetPlayerMmRsAsync(Arg.Any<IEnumerable<IUser>>(), MatchFormat.Ranked).Returns(players);

            MatchEntity result = await _sut.CreateMatchFromQueueAsync(new(MatchFormat.Ranked), default);

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
