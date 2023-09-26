using ApacBreachersRanked.Application.MMR.Commands;
using ApacBreachersRanked.Config;
using Discord.WebSocket;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ApacBreachersRanked.Services
{
    public class DiscordStartupService : IHostedService
    {
        private readonly DiscordSocketClient _discord;
        private readonly IMediator _mediator;
        private readonly DiscordOptions _config;
        private readonly ILogger<DiscordStartupService> _logger;

        public DiscordStartupService(DiscordSocketClient discord, IMediator mediator, IOptions<DiscordOptions> config, ILogger<DiscordStartupService> logger)
        {
            _discord = discord;
            _mediator = mediator;
            _config = config.Value;
            discord.Log += Log;
            discord.Ready += OnReadyAsync;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _discord.LoginAsync(Discord.TokenType.Bot, _config.Token);
            await _discord.StartAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _discord.LogoutAsync();
            await _discord.StopAsync();
        }

        private async Task Log(Discord.LogMessage msg)
        {
            switch (msg.Severity.ToString())
            {
                case "Critical":
                    {
                        _logger.LogCritical(msg.Exception, msg.ToString());
                        break;
                    }
                case "Warning":
                    {
                        _logger.LogWarning(msg.Exception, msg.ToString());
                        break;
                    }
                case "Info":
                    {
                        _logger.LogInformation(msg.ToString());
                        break;
                    }
                case "Verbose":
                    {
                        _logger.LogInformation(msg.ToString());
                        break;
                    }
                case "Debug":
                    {
                        _logger.LogDebug(msg.ToString());
                        break;
                    }
                case "Error":
                    {
                        _logger.LogError(msg.Exception, msg.ToString());
                        break;
                    }
            }
        }

        private async Task OnReadyAsync()
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await _mediator.Send(new SetRankEmojisCommand());
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
