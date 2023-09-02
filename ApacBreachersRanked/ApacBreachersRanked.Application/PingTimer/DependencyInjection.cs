using ApacBreachersRanked.Application.PingTimer.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ApacBreachersRanked.Application.PingTimer
{
    internal static class DependencyInjection
    {
        internal static IServiceCollection AddPingTimer(this IServiceCollection services)
        {
            services.AddSingleton<PingTimerService>();

            services.AddHostedService(serviceProvider => serviceProvider.GetRequiredService<PingTimerService>());

            return services;
        }
    }
}
