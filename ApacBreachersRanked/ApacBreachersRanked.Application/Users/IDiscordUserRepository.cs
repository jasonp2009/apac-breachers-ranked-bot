using ApacBreachersRanked.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacBreachersRanked.Application.Users
{
    public interface IDiscordUserRepository : IRepository<DiscordUser>
    {
    }
}
