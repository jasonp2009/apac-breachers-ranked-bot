using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacBreachersRanked.Domain.Interfaces
{
    public interface IUserService
    {
        public Task<IUser> GetUserAsync(IUserId userId);
    }
}
