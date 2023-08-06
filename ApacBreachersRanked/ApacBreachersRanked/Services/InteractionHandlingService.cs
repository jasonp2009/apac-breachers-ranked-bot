﻿using ApacBreachersRanked.Application.MatchQueue.Commands;
using ApacBreachersRanked.Domain.Match.Enums;
using ApacBreachersRanked.Domain.MMR.Events;
using ApacBreachersRanked.TypeConverters;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Example.Services
{
    public class InteractionHandlingService : IHostedService
    {
        private readonly DiscordSocketClient _discord;
        private readonly InteractionService _interactions;
        private readonly IServiceProvider _services;
        private readonly ILogger<InteractionHandlingService> _logger;

        public InteractionHandlingService(
            DiscordSocketClient discord,
            InteractionService interactions,
            IServiceProvider services,
            ILogger<InteractionHandlingService> logger)
        {
            _discord = discord;
            _interactions = interactions;
            _services = services;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _discord.Ready += () => _interactions.RegisterCommandsGloballyAsync(true);
            _discord.InteractionCreated += OnInteractionAsync;
            _discord.Ready += OnReadyAsync;

            _interactions.AddTypeConverter<Map>(new EnumConverter<Map>());

            await _interactions.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _interactions.Dispose();
            return Task.CompletedTask;
        }

        private async Task OnInteractionAsync(SocketInteraction interaction)
        {
            _logger.BeginScope("User {UserName}:({UserId}) performing {InteractionType}",
                interaction.User.Username, interaction.User.Id, interaction.Type);
            try
            {
                var context = new SocketInteractionContext(_discord, interaction);
                var result = await _interactions.ExecuteCommandAsync(context, _services);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning(result.ErrorReason);
                }
            }
            catch
            {
                if (interaction.Type == InteractionType.ApplicationCommand)
                {
                    await await interaction.GetOriginalResponseAsync()
                        .ContinueWith(async msg => await msg.Result.DeleteAsync());
                }
            }
        }

        private async Task OnReadyAsync()
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    using (IServiceScope scope = _services.CreateScope())
                    {
                        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        await mediator.Send(new InitialiseQueueCommand());
                        await mediator.Publish(new MatchMMRCalculatedEvent());
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, "An exception occured when processing on ready tasks");
                    throw;
                }
            });
        }
    }
}