using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.PingTimer.Events;
using ApacBreachersRanked.Application.PingTimer.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ApacBreachersRanked.Application.PingTimer.Services
{
    internal class PingTimerService : BackgroundService
    {
        private PeriodicTimer? _timer = null;
        private readonly IServiceProvider _services;
        private readonly ILogger<PingTimerService> _logger;
        private CancellationToken _stoppingToken;

        public PingTimerService(IServiceProvider services, ILogger<PingTimerService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timer = new(TimeSpan.FromSeconds(30));
            _stoppingToken = stoppingToken;

            while (await _timer.WaitForNextTickAsync(stoppingToken))
            {
                await CheckExpiredTimers();
            }
        }

        private async Task CheckExpiredTimers()
        {
            using (IServiceScope scope = _services.CreateScope())
            {
                IDbContext dbContext = scope.ServiceProvider.GetRequiredService<IDbContext>();
                List<TimedPing> expiredTimers = await dbContext.TimedPings
                    .Where(x => x.IsTimedOut && x.NextPingUtc < DateTime.UtcNow)
                    .ToListAsync(_stoppingToken);

                if (!expiredTimers.Any()) return;

                IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                foreach (TimedPing expiredTimer in expiredTimers)
                {
                    await mediator.Publish(new RoleTimerExpiredEvent { RoleId = expiredTimer.RoleId }, _stoppingToken);
                }
            }
        }
    }
}
