using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Infrastructure.Config;
using ApacBreachersRanked.Infrastructure.MatchQueueListener;
using ApacBreachersRanked.Infrastructure.Persistance;
using ApacBreachersRanked.Infrastructure.ScheduledEventHandling;
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

            services.AddScoped<BreachersDbContext>();

            services.AddScoped<IDbContext, BreachersDbContext>();

            services.AddSingleton<ScheduledEventHandlingService>();

            services.AddHostedService(serviceProvider => serviceProvider.GetRequiredService<ScheduledEventHandlingService>());

            services.AddSingleton<MatchQueueListenerService>();

            services.AddHostedService(serviceProvider => serviceProvider.GetRequiredService<MatchQueueListenerService>());

            return services;
        }
    }
}
