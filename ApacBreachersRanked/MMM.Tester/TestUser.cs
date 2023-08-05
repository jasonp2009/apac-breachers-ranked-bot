using ApacBreachersRanked.Domain.User.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMM.Tester
{
    internal class TestUser : IUser
    {
        public IUserId UserId { get; set; } = new TestUserId();

        public string? Name { get; set; }
    }

    internal class TestUserId : IUserId
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public bool Equals(IUserId rhs)
        {
            if (rhs is TestUserId testUserId)
            {
                return Id == testUserId.Id;
            }
            return false;
        }
    }
}
