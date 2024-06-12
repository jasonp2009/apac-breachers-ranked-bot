using ApacBreachersRanked.Application.BreachersUsers.Services;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Infrastructure.Breachers.Api;
using ApacBreachersRanked.Infrastructure.Breachers.Services;
using ApacBreachersRanked.Infrastructure.Config;
using ApacBreachersRanked.Infrastructure.MatchQueueListener;
using ApacBreachersRanked.Infrastructure.Persistance;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApacBreachersRanked.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            
            services.Configure<RdsOptions>(options => configuration.GetSection(RdsOptions.Key).Bind(options));

            services.AddScoped<BreachersDbContext>();

            services.AddScoped<IDbContext, BreachersDbContext>();

            services.AddScoped<IBreachersUserService, BreachersUserService>();
            
            services.AddBreachersApi(configuration);
            
            return services;
        }

        public static IServiceCollection AddMatchQueueListenderService(this IServiceCollection services)
        {
            services.AddSingleton<MatchQueueListenerService>();

            services.AddHostedService(serviceProvider => serviceProvider.GetRequiredService<MatchQueueListenerService>());

            return services;
        }

        private static IServiceCollection AddBreachersApi(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<BreachersApiOptions>(options => configuration.GetSection(BreachersApiOptions.Key).Bind(options));

            services.AddTransient<BreachersAuthenticationHandler>();
            
            services.AddHttpClient<BreachersApiClient>()
                .AddHttpMessageHandler<BreachersAuthenticationHandler>();
            return services;
        }
    }
}
