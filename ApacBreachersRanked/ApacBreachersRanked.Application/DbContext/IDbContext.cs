using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacBreachersRanked.Application.DbContext
{
    public partial interface IDbContext
    {
        public EntityEntry Attach(object obj);
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
