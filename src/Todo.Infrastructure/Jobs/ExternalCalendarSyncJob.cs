using HomeApps.Plugins.Notifications;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Todo.Application;
using Todo.Application.Abstractions;
using Todo.Application.ExternalCalendars;
using Todo.Infrastructure.Persistence;

namespace Todo.Infrastructure.Jobs;

/// <summary>
/// ADR 3.2/3.8: every few minutes refreshes the imported calendars that were last tried half an hour ago, skipping
/// those whose owner left. After three days of failures the owner is told once. The outbox is committed with an explicit
/// <c>SaveChangesAsync</c> after the notification (ADR decision 14).
/// </summary>
internal sealed partial class ExternalCalendarSyncJob(IServiceScopeFactory scopes, ILogger<ExternalCalendarSyncJob> logger) : BackgroundService
{
    public static readonly TimeSpan Interval = TimeSpan.FromMinutes(5);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Interval);
        do
        {
            await RunOnceAsync(stoppingToken);
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    internal async Task RunOnceAsync(CancellationToken cancellationToken)
    {
        try
        {
            await using var scope = scopes.CreateAsyncScope();
            var sp = scope.ServiceProvider;
            var time = sp.GetRequiredService<TimeProvider>();
            var due = await sp.GetRequiredService<IExternalCalendarRepository>().ListDueForSyncAsync(time.GetUtcNow(), cancellationToken);
            var syncer = sp.GetRequiredService<ExternalCalendarSyncer>();
            var notifier = sp.GetRequiredService<INotifier>();
            var db = sp.GetRequiredService<TodoDbContext>();

            foreach (var calendar in due)
            {
                try
                {
                    await syncer.SyncAsync(calendar, cancellationToken);
                    if (calendar.ShouldNotifyOwner)
                    {
                        await notifier.NotifyAsync(new PluginNotification(
                            [calendar.UserId], TodoPlugin.CalendarSyncNotification, "Kalendarz nie odświeża się",
                            $"Nie udało się pobrać kalendarza „{calendar.Name}” od kilku dni.",
                            $"/homes/{calendar.HouseholdId}/apps/todo", calendar.HouseholdId, $"calendar-sync-{calendar.Id}"), cancellationToken);
                    }

                    await db.SaveChangesAsync(cancellationToken);
                }
                catch (Exception exception) when (exception is not OperationCanceledException)
                {
                    LogFailed(exception);
                }
            }
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            LogFailed(exception);
        }
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Synchronising imported calendars failed; trying again later.")]
    private partial void LogFailed(Exception exception);
}
