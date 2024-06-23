using ApacBreachersRanked.Config;
using ApacBreachersRanked.Services;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Example.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApacBreachersRanked
{
    public static class ConfigureDiscordBot
    {
        public static IServiceCollection AddInteractionService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(new InteractionServiceConfig
            {
                AutoServiceScopes = false,
                DefaultRunMode = RunMode.Sync,
                ThrowOnError = true
            });
            services.AddSingleton<InteractionService>(x => new InteractionService(
                x.GetRequiredService<DiscordSocketClient>(),
                x.GetRequiredService<InteractionServiceConfig>()));
            services.AddHostedService<InteractionHandlingService>();

            return services;
        }

        public static IServiceCollection AddDiscordClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DiscordOptions>(options => configuration.GetSection(DiscordOptions.Key).Bind(options));

            services.AddSingleton(new DiscordSocketClient(
                new DiscordSocketConfig
                {
                    LogGatewayIntentWarnings = false
                }
            ));
            services.AddSingleton<IDiscordClient>(x => x.GetRequiredService<DiscordSocketClient>());
            services.AddHostedService<DiscordStartupService>();
            return services;
        }
    }
}
