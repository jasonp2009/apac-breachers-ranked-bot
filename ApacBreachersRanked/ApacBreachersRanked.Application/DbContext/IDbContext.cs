using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacBreachersRanked.Application.DbContext
{
    public interface IDbContext
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
