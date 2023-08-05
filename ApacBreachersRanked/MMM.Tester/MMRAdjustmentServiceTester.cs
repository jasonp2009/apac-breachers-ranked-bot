using ApacBreachersRanked.Domain.MMR;
using ApacBreachersRanked.Domain.MMR.Entities;
using ApacBreachersRanked.Domain.MMR.Services;
using ApacBreachersRanked.Domain.User.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMM.Tester
{
    internal class MMRAdjustmentServiceTester : IMMRAdjustmentService
    {
        private readonly MMRConfig _config;
        public MMRAdjustmentServiceTester(MMRConfig config)
        {
            _config = config;
        }

        MMRConfig IMMRAdjustmentService._config => _config;

        Task<List<PlayerMMR>> IMMRAdjustmentService.GetPlayerMMRsAsync(IEnumerable<IUser> users, CancellationToken cancellationToken)
            => Task.FromResult(users.Select(user => new PlayerMMR(user)).ToList());
    }
}
