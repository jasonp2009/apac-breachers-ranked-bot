using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApacBreachersRanked.Domain.Entities;

namespace ApacBreachersRanked.Domain.Interfaces
{
    public interface IUserId
    {
        public bool Equals(IUserId rhs);
    }

    public interface IUser
    {
        public IUserId UserId { get; }
        public string Name { get; }
        public bool Equals(IUser rhs)
        {
            return UserId.Equals(rhs.UserId);
        }
    }
}
