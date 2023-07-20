using ApacBreachersRanked.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacBreachersRanked.Application.Users
{
    public class DiscordUser : User
    {
        public ulong DiscordUserId { get; set; }
    }
}
