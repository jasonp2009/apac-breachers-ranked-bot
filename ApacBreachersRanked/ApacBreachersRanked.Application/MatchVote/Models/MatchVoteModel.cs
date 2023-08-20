using ApacBreachersRanked.Application.MatchVote.Constants;
using ApacBreachersRanked.Application.MatchVote.Enums;
using ApacBreachersRanked.Application.MatchVote.Events;
using ApacBreachersRanked.Domain.Common;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.Match.Enums;
using ApacBreachersRanked.Domain.User.Interfaces;

namespace ApacBreachersRanked.Application.MatchVote.Models
{
    public class MatchVoteModel : BaseEntity
    {
        public Guid MatchId { get; private set; }
        public MatchEntity Match { get; private set; }
        public MatchVoteStatus Status { get; private set; }
        public List<MatchVoteMap> HomeVotes { get; private set; } = new();
        public DateTime HomeVoteEndUtc { get; private set; } = DateTime.UtcNow + TimeSpan.FromSeconds(MatchVoteConstants.VoteTimeSeconds);
        public ulong? HomeVoteMessageId { get; set; } = null;
        public Map? VotedMap { get; private set; } = null;
        public List<MatchVoteSide> AwayVotes { get; private set; } = new();
        public DateTime? AwayVoteEndUtc { get; private set; } = null;
        public ulong? AwayVoteMessageId { get; set; } = null;
        public GameSide? VotedHomeSide { get; private set; } = null;

        private MatchVoteModel()
        {

        }

        public MatchVoteModel(MatchEntity match)
        {
            MatchId = match.Id;
            Match = match;
            HomeVotes = match.HomePlayers.Select(x => new MatchVoteMap(x)).ToList();
            AwayVotes = match.AwayPlayers.Select(x => new MatchVoteSide(x)).ToList();
            QueueDomainEvent(new MatchVoteCreatedEvent { MatchId = match.Id });
            QueueDomainEvent(new MapVoteTimedOutEvent { ScheduledForUtc = HomeVoteEndUtc, MatchId = match.Id });
        }

        public void CastMapVote(IUserId userId, Map vote)
        {
            MatchVoteMap? user = HomeVotes.FirstOrDefault(x => x.UserId.Equals(userId));
            if (user != null)
            {
                user.SetVote(vote);
                QueueDomainEvent(new MapVoteUpdatedEvent { MatchId = MatchId });
            }
            if (HomeVotes.All(x => x.Vote != null))
            {
                CompletMapVote();
            }
        }

        public void CompletMapVote()
        {
            if (Status != MatchVoteStatus.VotingOnMap) return;
            // Count votes for each map
            var voteCount = new Dictionary<Map, int>();
            foreach (Map map in Enum.GetValues(typeof(Map)).Cast<Map>())
            {
                voteCount.Add(map, 0);
            }
            foreach (var voteMap in HomeVotes.Where(x => x.Vote != null))
            {
                voteCount[voteMap.Vote!.Value]++;
            }

            // Select maps with the most votes
            int maxVotes = voteCount.Values.Max();
            var mostVotedMaps = voteCount.Where(kv => kv.Value == maxVotes).Select(kv => kv.Key).ToList();

            // Select a map at random from the most voted maps
            Random random = new Random();
            int randomIndex = random.Next(mostVotedMaps.Count);

            VotedMap = mostVotedMaps[randomIndex];
            Status = MatchVoteStatus.VotingOnSide;
            AwayVoteEndUtc = DateTime.UtcNow + TimeSpan.FromSeconds(MatchVoteConstants.VoteTimeSeconds);
            QueueDomainEvent(new MapVoteCompletedEvent { MatchId = MatchId });
            QueueDomainEvent(new SideVoteTimedOutEvent { ScheduledForUtc = AwayVoteEndUtc.Value, MatchId = MatchId });
        }

        public void CastSideVote(IUserId userId, GameSide vote)
        {
            MatchVoteSide? user = AwayVotes.FirstOrDefault(x => x.UserId.Equals(userId));
            if (user != null)
            {
                user.SetVote(vote);
                QueueDomainEvent(new SideVoteUpdatedEvent { MatchId = MatchId });
            }
            if (AwayVotes.All(x => x.Vote != null))
            {
                CompletSideVote();
            }
        }

        public void CompletSideVote()
        {
            if (Status != MatchVoteStatus.VotingOnSide) return;

            // Count votes for each side
            var voteCount = new Dictionary<GameSide, int>();
            foreach (GameSide side in Enum.GetValues(typeof(GameSide)).Cast<GameSide>())
            {
                voteCount.Add(side, 0);
            }
            foreach (var voteMap in AwayVotes.Where(x => x.Vote != null))
            {
                voteCount[voteMap.Vote!.Value]++;
            }

            // Select maps with the most votes
            int maxVotes = voteCount.Values.Max();
            var mostVotedMaps = voteCount.Where(kv => kv.Value == maxVotes).Select(kv => kv.Key).ToList();

            // Select a map at random from the most voted maps
            Random random = new Random();
            int randomIndex = random.Next(mostVotedMaps.Count);

            if (mostVotedMaps[randomIndex] == GameSide.Enforcers)
            {
                VotedHomeSide = GameSide.Revolters;
            } else
            {
                VotedHomeSide = GameSide.Enforcers;
            }
            Status = MatchVoteStatus.VotingComplete;
            QueueDomainEvent(new SideVoteCompletedEvent { MatchId = MatchId });
        }
    }
}
