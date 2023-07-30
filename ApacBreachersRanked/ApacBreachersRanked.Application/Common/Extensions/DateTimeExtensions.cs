using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacBreachersRanked.Application.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static int ToEpoch(this DateTime utcDateTime)
        {
            TimeSpan t = utcDateTime - new DateTime(1970, 1, 1);
            return (int)t.TotalSeconds;
        }
    }
}
