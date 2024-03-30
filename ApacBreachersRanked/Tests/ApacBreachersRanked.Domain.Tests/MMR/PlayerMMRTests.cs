using ApacBreachersRanked.Domain.Match.Enums;
using ApacBreachersRanked.Domain.MMR.Entities;
using ApacBreachersRanked.Domain.MMR.Enums;
using ApacBreachersRanked.Domain.MMR.Events;
using ApacBreachersRanked.Domain.Tests.User;
using ApacBreachersRanked.Domain.User.Interfaces;
using ApacBreachersRanked.Infrastructure.SQS.Extensions;
using FluentAssertions;

namespace ApacBreachersRanked.Domain.Tests.MMR
{
    public class PlayerMMRTests
    {
        [Theory]
        [InlineData(Rank.Silver, 1029, 10, Rank.Silver)]
        [InlineData(Rank.Silver, 1030, 10, Rank.Gold)]
        [InlineData(Rank.Silver, 971, -10, Rank.Silver)]
        [InlineData(Rank.Silver, 961, -10, Rank.Bronze)]
        public void GivenRankMMRAndAdjustment_WhenAdjustingMMR_AdjustRankAsExpected(Rank rank, decimal MMR, decimal adjustment, Rank expectedRank)
        {
            // ARRANGE
            IUser testUser = new TestUser();

            PlayerMMR playerMMR = new PlayerMMR(testUser, MatchFormat.Ranked, MMR, rank);

            MMRAdjustment mmrAdjustment = new MMRAdjustment(testUser.UserId, MatchFormat.Ranked, adjustment, null!);

            // ACT
            playerMMR.ApplyAdjustment(mmrAdjustment);

            // ASSERT
            playerMMR.Rank.Should().Be(expectedRank);
        }

        [Fact]
        public void Test()
        {
            MatchMMRCalculatedEvent test = new()
            {
                MatchId = Guid.Parse("DDC4090E-DAD8-44CD-9283-E01D63403EBF")
            };
            var testString = MessageSerializer.Serialize(test);
            Console.WriteLine(testString);
        }
    }
}
