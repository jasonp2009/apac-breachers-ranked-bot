using ApacBreachersRanked.Domain.Match.Services;
using Microsoft.Extensions.DependencyInjection;

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
