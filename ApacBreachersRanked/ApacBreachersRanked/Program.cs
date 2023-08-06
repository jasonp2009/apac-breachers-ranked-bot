using ApacBreachersRanked;
using ApacBreachersRanked.Application;
using ApacBreachersRanked.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((host,services) =>
    {
        services.AddDiscordBot(host.Configuration);
        services.AddApplication(host.Configuration);
        services.AddInfrastructure(host.Configuration);
        services.AddLogging(cfg =>
        {
            cfg.SetMinimumLevel(LogLevel.Debug)
                .AddConsole();
        });
    })
    .Build();

await host.RunAsync();