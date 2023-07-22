using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacBreachersRanked.Application.Config
{
    public class BreachersDiscordOptions
    {
        public static string Key = nameof(BreachersDiscordOptions);
        public ulong GuildId { get; init; }
        public ulong ReadyUpChannelId { get; init; }
    }
}
