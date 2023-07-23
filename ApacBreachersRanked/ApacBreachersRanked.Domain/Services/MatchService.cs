using ApacBreachersRanked.Domain.Constants;
using ApacBreachersRanked.Domain.Entities;
using ApacBreachersRanked.Domain.Helpers;
using ApacBreachersRanked.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacBreachersRanked.Domain.Services
{
    internal class MatchService : IMatchService
    {
        private readonly IUserService _userService;

        public MatchService(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<MatchEntity> CreateMatchFromQueue(MatchQueueEntity matchQueue)
        {
            IEnumerable<Task<IUser>> userTasks = matchQueue.Users
                .Take(MatchConstants.MaxCapacity)
                .Select(x => _userService.GetUserAsync(x.UserId));

            await Task.WhenAll(userTasks);

            List<IUser> users = userTasks.Select(userTask => userTask.Result).ToList();

            (List<IUser> home, List<IUser> away) = RandomiseTeams(users);

            return new MatchEntity(matchQueue, home, away);

        }

        public (List<IUser> Home, List<IUser> Away) RandomiseTeams(List<IUser> users)
        {
            users.Shuffle();

            List<IUser> home = new(), away = new();

            bool selectHome = true;

            foreach(var user in users)
            {
                if (selectHome)
                {
                    home.Add(user);
                } else
                {
                    away.Add(user);
                }
                selectHome = !selectHome;
            }
            return (home, away);
        }
    }
}
