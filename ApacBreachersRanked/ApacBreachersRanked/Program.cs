﻿using ApacBreachersRanked;
using ApacBreachersRanked.Application;
using ApacBreachersRanked.Infrastructure;
using ApacBreachersRanked.Infrastructure.SQS;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Formatting.Compact;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((host,services) =>
    {
        services.UseSqsPublisher(host.Configuration);
        services.AddInteractionService(host.Configuration);
        services.AddDiscordClient(host.Configuration);
        services.AddApplication(host.Configuration);
        services.AddInfrastructure(host.Configuration);
    })
    .UseSerilog((a,cfg) =>
    {
        cfg.WriteTo.Console(new CompactJsonFormatter());
    })
    .Build();

await host.RunAsync();