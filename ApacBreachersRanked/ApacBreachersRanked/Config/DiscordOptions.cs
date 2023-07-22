using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacBreachersRanked.Config
{
    public class DiscordOptions
    {
        public static string Key = nameof(DiscordOptions);
        public string Token { get; init; } = null!;
    }
}
