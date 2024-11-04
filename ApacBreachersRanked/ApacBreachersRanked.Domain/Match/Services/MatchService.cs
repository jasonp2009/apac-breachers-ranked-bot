using ApacBreachersRanked.Domain.Helpers;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.MatchQueue.Entities;
using ApacBreachersRanked.Domain.MMR.Entities;
using ApacBreachersRanked.Domain.MMR.Services;
using ApacBreachersRanked.Domain.User.Interfaces;

namespace ApacBreachersRanked.Domain.Match.Services
{
    public class MatchService : IMatchService
    {
        private readonly IMmrService _mmrService;

        public MatchService(IMmrService mmrService)
        {
            _mmrService = mmrService;
        }

        public async Task<MatchEntity> CreateMatchFromQueueAsync(MatchQueueEntity matchQueue, CancellationToken cancellationToken)
        {
            List<IUser> users = matchQueue.Users
                .OrderBy(x => x.JoinedAtUtc)
                .Take(matchQueue.MatchFormat.GetMatchConstant(c => c.MaxCapacity))
                .Select(user => user as IUser)
                .ToList();

            List<PlayerMMR> playerMMRs = await _mmrService.GetPlayerMmRsAsync(users, matchQueue.MatchFormat, cancellationToken: cancellationToken);

            playerMMRs = (await GetNewPlayedAdjustedMmrsAsync(playerMMRs, cancellationToken)).ToList();

            (List<PlayerMMR> home, List<PlayerMMR> away) = AllocateTeams(playerMMRs);

            return new MatchEntity(matchQueue, home, away);
        }

        public async Task<IEnumerable<PlayerMMR>> GetNewPlayedAdjustedMmrsAsync(IEnumerable<PlayerMMR> playerMmrs, CancellationToken cancellationToken)
        {
            var matchesPlayed = await _mmrService.GetMatchesPlayedAsync(playerMmrs, cancellationToken);
            return matchesPlayed.Select(pair =>
            {
                if (pair.Value >= 5) return pair.Key;

                var curMmr = pair.Key;

                return new PlayerMMR(
                    curMmr,
                    curMmr.MatchFormat,
                    curMmr.MMR - (5 - pair.Value) * 10,
                    curMmr.Rank);
            });
        }

        public (List<PlayerMMR> Home, List<PlayerMMR> Away) AllocateTeams(List<PlayerMMR> playerMMRs)
        {
            playerMMRs = playerMMRs.OrderByDescending(x => x.MMR).ToList();
            List<Tuple<PlayerMMR, PlayerMMR>> pairs = new();
            List<bool> rotations = new();
            while (playerMMRs.Count > 1)
            {
                pairs.Add(new(playerMMRs[0], playerMMRs[1]));
                rotations.Add(false);
                playerMMRs.RemoveRange(0, 2);
            }
            List<bool> bestRotation = new();
            decimal bestMMR = decimal.MaxValue;

            CheckRotations(pairs, rotations, 0, ref bestRotation, ref bestMMR);
            List<PlayerMMR> home = new();
            List<PlayerMMR> away = new();
            for(int i = 0; i < pairs.Count; i++)
            {
                if (bestRotation[i])
                {
                    home.Add(pairs[i].Item1);
                    away.Add(pairs[i].Item2);
                } else
                {
                    home.Add(pairs[i].Item2);
                    away.Add(pairs[i].Item1);
                }
            }

            if (playerMMRs.Any())
            {
                var remainingPlayer = playerMMRs.FirstOrDefault();
                if (home.Sum(x => x.MMR) > away.Sum(x => x.MMR))
                {
                    away.Add(remainingPlayer);
                }
                else
                {
                    home.Add(remainingPlayer);
                }
            }
            return (home, away);
        }

        private void CheckRotations(
            List<Tuple<PlayerMMR, PlayerMMR>> pairs,
            List<bool> rotations,
            int rotationIndex,
            ref List<bool> bestRotations,
            ref decimal bestMMR)
        {
            if (rotationIndex < rotations.Count)
            {
                CheckRotations(pairs, new List<bool>(rotations), rotationIndex + 1, ref bestRotations, ref bestMMR);
                List<bool> rotatedRotations = new List<bool>(rotations);
                rotatedRotations[rotationIndex] = !rotatedRotations[rotationIndex];
                CheckRotations(pairs, rotatedRotations, rotationIndex + 1, ref bestRotations, ref bestMMR);
                return;
            }
            decimal teamA = 0, teamB = 0;
            for(int i = 0; i < pairs.Count; i++)
            {
                var pair = pairs[i];
                if (rotations[i])
                {
                    teamA += pair.Item1.MMR;
                    teamB += pair.Item2.MMR;
                } else
                {

                    teamA += pair.Item2.MMR;
                    teamB += pair.Item1.MMR;
                }
            }
            decimal curDiff = Math.Abs(teamA - teamB);
            if (curDiff < bestMMR)
            {
                bestRotations = rotations;
                bestMMR = curDiff;
            }
        }
    }
}
