using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacBreachersRanked.Domain.MMR
{
    public class MMRConfig
    {
        public decimal KFactor { get; init; }
        public decimal MapWeighting { get; init; }
        public decimal RoundWeighting { get; init; }
    }
}
