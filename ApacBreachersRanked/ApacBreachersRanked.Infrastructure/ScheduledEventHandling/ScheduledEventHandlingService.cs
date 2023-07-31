using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Domain.Common;
using ApacBreachersRanked.Infrastructure.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ApacBreachersRanked.Infrastructure.ScheduledEventHandling
{
    internal class ScheduledEventHandlingService : IHostedService, IDisposable
    {
        private Timer? _timer = null;
        private readonly IServiceProvider _services;

        public ScheduledEventHandlingService(IServiceProvider services)
        {
            _services = services;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(
                async _ => await DoWorkAsync(),
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(10));

            return Task.CompletedTask;
        }

        private async Task DoWorkAsync()
        {
            using (IServiceScope scope = _services.CreateScope())
            {
                BreachersDbContext dbContext = scope.ServiceProvider.GetRequiredService<BreachersDbContext>();
                

                List<ScheduledEvent> events = await dbContext.ScheduledEvents.Where(x => x.ScheduledForUtc <= DateTime.UtcNow).ToListAsync();
                
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
                        catch (Exception)
                        {

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
                IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                await mediator.Publish(scheduledEvent, default);
            }
        }

        public async Task ScheduleEvent(IScheduledEvent scheduledEvent)
        {
            using (IServiceScope scope = _services.CreateScope())
            {
                try
                {
                    BreachersDbContext dbContext = scope.ServiceProvider.GetRequiredService<BreachersDbContext>();
                    await dbContext.AddAsync(new ScheduledEvent(scheduledEvent));
                    await dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
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
