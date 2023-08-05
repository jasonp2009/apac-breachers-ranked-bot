using ApacBreachersRanked.Application.MatchQueue.Commands;
using ApacBreachersRanked.Domain.Match.Enums;
using ApacBreachersRanked.Domain.MMR.Events;
using ApacBreachersRanked.TypeConverters;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace Example.Services
{
    public class InteractionHandlingService : IHostedService
    {
        private readonly DiscordSocketClient _discord;
        private readonly InteractionService _interactions;
        private readonly IServiceProvider _services;

        public InteractionHandlingService(
            DiscordSocketClient discord,
            InteractionService interactions,
            IServiceProvider services)
        {
            _discord = discord;
            _interactions = interactions;
            _services = services;
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
            try
            {
                var context = new SocketInteractionContext(_discord, interaction);
                var result = await _interactions.ExecuteCommandAsync(context, _services);
                if (!result.IsSuccess)
                {
                    Console.WriteLine(result.ErrorReason);
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
                    Console.WriteLine(ex);
                    throw;
                }
                
            });
        }
    }
}