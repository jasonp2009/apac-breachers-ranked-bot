using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Domain.Common;
using ApacBreachersRanked.Infrastructure.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ApacBreachersRanked.Infrastructure.ScheduledEventHandling
{
    internal class ScheduledEventHandlingService : IHostedService, IDisposable
    {
        private Timer? _timer = null;
        private readonly IServiceProvider _services;
        private readonly ILogger<ScheduledEventHandlingService> _logger;

        public ScheduledEventHandlingService(IServiceProvider services, ILogger<ScheduledEventHandlingService> logger)
        {
            _services = services;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(
                async _ => await DoWorkAsync(),
                null,
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(10));

            return Task.CompletedTask;
        }

        private async Task DoWorkAsync()
        {
            using (IServiceScope scope = _services.CreateScope())
            {
                BreachersDbContext dbContext = scope.ServiceProvider.GetRequiredService<BreachersDbContext>();

                List<ScheduledEvent> events = await dbContext.ScheduledEvents.Where(x => x.ScheduledForUtc <= DateTime.UtcNow).ToListAsync();

                if (events.Count == 0) return;

                foreach (var scheduledEvent in events)
                {
                    if (scheduledEvent.ScheduledForUtc <= DateTime.UtcNow)
                    {
                        try
                        {
                            if (scheduledEvent.Event != null)
                            {
                                await HandleEvent(scheduledEvent.Event);
                            }

                            dbContext.ScheduledEvents.Remove(scheduledEvent);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "An exception occurred when trying to DoWorkAsync()");
                        }
                    }
                }
                await dbContext.SaveChangesAsync();
            }
        }

        private async Task HandleEvent(IScheduledEvent scheduledEvent)
        {
            using (IServiceScope scope = _services.CreateScope())
            {
                try
                {
                    IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    await mediator.Publish(scheduledEvent, default);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An exception occurent when trying to HandleEvent()");
                }
            }
        }

        public async void ScheduleEvent(IDomainEvent domainEvent)
        {
            _ = Task.Run(async () =>
            {
                using (IServiceScope scope = _services.CreateScope())
                {
                    try
                    {
                        if (domainEvent is IScheduledEvent scheduledEvent)
                        {
                            BreachersDbContext dbContext = scope.ServiceProvider.GetRequiredService<BreachersDbContext>();
                            await dbContext.AddAsync(new ScheduledEvent(scheduledEvent));
                            await dbContext.SaveChangesAsync();
                        }
                        else
                        {
                            IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                            await mediator.Publish(domainEvent);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An exception occurent when trying to ScheduleEvent()");
                    }

                }
            });
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
