using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Linq;
using Event.Data;

public class EventStatusUpdateService : IHostedService, IDisposable
{
    private Timer _timer;
    private readonly IServiceProvider _serviceProvider;

    public EventStatusUpdateService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Hemen başla, her 24 saatte bir çalıştır
        _timer = new Timer(UpdateStatuses, null, TimeSpan.Zero, TimeSpan.FromHours(24));
        return Task.CompletedTask;
    }

    private void UpdateStatuses(object state)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var now = DateTime.Now;

        var eventsToUpdate = context.EventItem
            .Where(e => e.EventEndDate < now && e.Status == "Aktif")
            .ToList();

        if (eventsToUpdate.Count == 0)
            return;

        foreach (var ev in eventsToUpdate)
        {
            ev.Status = "Süresi Doldu";
        }

        context.SaveChanges();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
