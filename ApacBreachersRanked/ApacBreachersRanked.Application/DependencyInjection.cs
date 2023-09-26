using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.Config;
using ApacBreachersRanked.Application.MMR.Services;
using ApacBreachersRanked.Application.PingTimer;
using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain;
using ApacBreachersRanked.Domain.MMR.Services;
using ApacBreachersRanked.Domain.User.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApacBreachersRanked.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

            services.Configure<BreachersDiscordOptions>(options => configuration.GetSection(BreachersDiscordOptions.Key).Bind(options));

            services.AddScoped<IUserService, UserService>();

            services.AddScoped<IMMRAdjustmentService, MMRAdjustmentService>();

            services.AddScoped<IMMRService, MMRService>();

            services.AddPingTimer();

            services.AddDomain();

            return services;
        }
    }
}
