using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.Entities;
using ApacBreachersRanked.Domain.Repositories;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacBreachersRanked.Infrastructure.Repositories
{
    internal class UserRepository : Repository<DiscordUser>, IDiscordUserRepository
    {
        public UserRepository(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {
        }
    }
}
