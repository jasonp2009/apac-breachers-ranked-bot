using ApacBreachersRanked.Domain.Match.Interfaces;
using ApacBreachersRanked.Domain.Match.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacBreachersRanked.Domain
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDomain(this IServiceCollection services)
        {
            services.AddScoped<IMatchService, MatchService>();
            return services;
        }
    }
}
