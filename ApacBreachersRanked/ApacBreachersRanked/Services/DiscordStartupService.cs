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

        public DiscordStartupService(DiscordSocketClient discord, IOptions<DiscordOptions> config)
        {
            _discord = discord;
            _config = config.Value;
            discord.Log += Log;
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
            Console.WriteLine(msg.ToString());
        }
    }
}
