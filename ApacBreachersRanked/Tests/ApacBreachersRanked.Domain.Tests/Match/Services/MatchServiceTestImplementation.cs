using ApacBreachersRanked.Domain.Match.Services;
using ApacBreachersRanked.Domain.MMR.Entities;
using ApacBreachersRanked.Domain.User.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacBreachersRanked.Domain.Tests.Match.Services
{
    public class MatchServiceTestImplementation : IMatchService
    {
        Task<List<PlayerMMR>> IMatchService.GetPlayerMMRsAsync(IEnumerable<IUser> users, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
