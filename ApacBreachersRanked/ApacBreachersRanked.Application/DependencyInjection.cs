using ApacBreachersRanked.Application.Config;
using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain;
using ApacBreachersRanked.Domain.User.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ApacBreachersRanked.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
            });

            services.Configure<BreachersDiscordOptions>(options => configuration.GetSection(BreachersDiscordOptions.Key).Bind(options));

            services.AddScoped<IUserService, UserService>();

            services.AddDomain();

            return services;
        }
    }
}
