using ApacBreachersRanked.Config;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ApacBreachersRanked.Services
{
    public class DiscordStartupService : IHostedService
    {
        private readonly DiscordSocketClient _discord;
        private readonly DiscordOptions _config;
        private readonly ILogger<DiscordStartupService> _logger;

        public DiscordStartupService(DiscordSocketClient discord, IOptions<DiscordOptions> config, ILogger<DiscordStartupService> logger)
        {
            _discord = discord;
            _config = config.Value;
            discord.Log += Log;
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
    }
}
