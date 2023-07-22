using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacBreachersRanked.Infrastructure.Config
{
    internal class RdsOptions
    {
        public static string Key = "RdsOptions";
        public string UserName { get; init; } = null!;
        public string Password { get; init; } = null!;
        public string HostName { get; init; } = null!;
        public string DbName { get; init; } = null!;
        internal string ConnectionString => "Data Source=" + HostName + ";Database=" + DbName + ";User ID=" + UserName + ";Password=" + Password + ";";
    }
}
