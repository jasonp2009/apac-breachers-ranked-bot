using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Infrastructure.Config;
using ApacBreachersRanked.Infrastructure.Persistance;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ApacBreachersRanked.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RdsOptions>(options => configuration.GetSection(RdsOptions.Key).Bind(options));

            services.AddScoped<IDbContext, BreachersDbContext>();

            return services;
        }
    }
}
