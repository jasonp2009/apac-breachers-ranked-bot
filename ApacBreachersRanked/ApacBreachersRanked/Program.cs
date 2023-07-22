using ApacBreachersRanked;
using ApacBreachersRanked.Application;
using ApacBreachersRanked.Infrastructure;
using Microsoft.Extensions.Hosting;


using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((host,services) =>
    {
        services.AddDiscordBot(host.Configuration);
        services.AddApplication(host.Configuration);
        services.AddInfrastructure(host.Configuration);
    })
    .Build();

await host.RunAsync();