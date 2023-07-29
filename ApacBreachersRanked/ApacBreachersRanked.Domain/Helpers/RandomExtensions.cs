using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacBreachersRanked.Domain.Helpers
{
    public static class RandomExtensions
    {
        public static T SelectRandom<T>(this IEnumerable<T> values)
        {
            Random rnd = new();
            return values.ElementAt(rnd.Next(values.Count()));
        }

        public static int RandomNumber() => new Random().Next();
        public static int RandomNumber(int max) => new Random().Next(max);
        public static int RandomNumber(int min, int max) => new Random().Next(min, max);
    }
}
