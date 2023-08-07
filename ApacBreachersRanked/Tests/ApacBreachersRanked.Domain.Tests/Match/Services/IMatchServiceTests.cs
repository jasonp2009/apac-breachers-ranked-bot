using ApacBreachersRanked.Domain.Common;
using ApacBreachersRanked.Domain.Helpers;
using ApacBreachersRanked.Domain.Match.Services;
using ApacBreachersRanked.Domain.MMR.Entities;
using ApacBreachersRanked.Domain.Tests.Common;
using ApacBreachersRanked.Domain.Tests.Match.Services;
using ApacBreachersRanked.Domain.Tests.User;
using ApacBreachersRanked.Domain.User.Interfaces;
using AutoFixture;
using AutoFixture.Kernel;
using FluentAssertions;
using MediatR;
using NSubstitute;
using System.Collections.Generic;

namespace ApacBreachersRanked.Tests.Match.Services
{
    public class IMatchServiceTests
    {
        private readonly IMatchService _sut;

        public IMatchServiceTests()
        {
            _sut = new MatchServiceTestImplementation();
        }

        [Fact]
        public void GivenAListOfDuplcatePlayerMMRs_WhenAllocatingTeams_AllocateTeamsWithNoMMRDifference()
        {
            // ARRANGE
            
            List<PlayerMMR> playerMMRs = new();

            for(int i = RandomExtensions.RandomNumber(2,2); i >= 0; i--)
            {
                decimal mmr = RandomExtensions.RandomNumber(100, 2000);
                playerMMRs.Add(new PlayerMMR(new TestUser(), mmr));
                playerMMRs.Add(new PlayerMMR(new TestUser(), mmr));
            }


            // ACT
            var result = _sut.AllocateTeams(playerMMRs);

            // ASSERT
            decimal totalHomeMMR = playerMMRs.Where(x => result.Home.Any(player => player.UserId.Equals(x.UserId))).Sum(x => x.MMR);
            decimal totalAwayMMR = playerMMRs.Where(x => result.Away.Any(player => player.UserId.Equals(x.UserId))).Sum(x => x.MMR);

            totalHomeMMR.Should().Be(totalAwayMMR);
            
        }

        [Fact]
        public void TestBruteForceAgainstGreedy()
        {
            for(int rep = 0; rep < 100; rep++)
            {
                // ARRANGE
                List<PlayerMMR> playerMMRs = new();

                for (int i = RandomExtensions.RandomNumber(2, 100); i > 0; i--)
                {
                    decimal mmr = RandomExtensions.RandomNumber(750, 1500);
                    playerMMRs.Add(new PlayerMMR(new TestUser(), mmr));
                }



                decimal reasonableMaxDiff = Math.Abs(playerMMRs.OrderByDescending(x => x.MMR).Where((x, i) => i % 2 == 0).Average(x => x.MMR) - playerMMRs.OrderByDescending(x => x.MMR).Where((x, i) => i % 2 == 1).Average(x => x.MMR))*10;

                // ACT
                var greedyResult = _sut.AllocateTeams(playerMMRs);

                decimal totalHomeMMR = playerMMRs.Where(x => greedyResult.Home.Any(player => player.UserId.Equals(x.UserId))).Sum(x => x.MMR);
                decimal totalAwayMMR = playerMMRs.Where(x => greedyResult.Away.Any(player => player.UserId.Equals(x.UserId))).Sum(x => x.MMR);

                decimal greedyDiff = Math.Abs(totalHomeMMR - totalAwayMMR);

                // ASSERT
                greedyResult.Home.Should().HaveCountGreaterThanOrEqualTo(1);
                greedyResult.Away.Should().HaveCountGreaterThanOrEqualTo(1);
                greedyResult.Home.Should().HaveCountLessThanOrEqualTo(5);
                greedyResult.Away.Should().HaveCountLessThanOrEqualTo(5);

                greedyDiff.Should().BeLessThanOrEqualTo(Math.Max(reasonableMaxDiff, 10));
            }
        }
    }
}
