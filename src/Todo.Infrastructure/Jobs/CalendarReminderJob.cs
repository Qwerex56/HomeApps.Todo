using HomeApps.Plugins.Notifications;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Todo.Application.Abstractions;
using Todo.Application.Reminders;
using Todo.Domain.Reminders;
using Todo.Infrastructure.Persistence;

namespace Todo.Infrastructure.Jobs;

/// <summary>
/// ADR 3.8: every 60 seconds finds the reminders that came due since the last scan (event, per-user override and task
/// reminders), drops those over an hour late and notifies. <c>INotifier</c> only queues the message in the outbox, so the
/// context is saved explicitly after the notifications (ADR decision 14).
/// </summary>
internal sealed partial class CalendarReminderJob(IServiceScopeFactory scopes, ILogger<CalendarReminderJob> logger) : BackgroundService
{
    public static readonly TimeSpan Interval = TimeSpan.FromSeconds(60);

    private DateTimeOffset? _lastScan;

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
        DateTimeOffset? scanned = null;
        try
        {
            await using var scope = scopes.CreateAsyncScope();
            var sp = scope.ServiceProvider;
            var now = sp.GetRequiredService<TimeProvider>().GetUtcNow();
            var from = _lastScan ?? now.AddMinutes(-1);
            var sources = await sp.GetRequiredService<IReminderReader>().LoadAsync(from, now.Add(ReminderScanner.Horizon), cancellationToken);
            var scan = ReminderScanner.Scan(sources, from, now, sp.GetService<IConfiguration>()?["Planner:DefaultTimeZone"] ?? ReminderScanner.DefaultZoneId);
            var notifier = sp.GetRequiredService<INotifier>();
            var db = sp.GetRequiredService<TodoDbContext>();

            foreach (var reminder in scan.Due)
            {
                await notifier.NotifyAsync(new PluginNotification(
                    [reminder.UserId], reminder.Category, reminder.Title, reminder.Body,
                    $"/homes/{reminder.HouseholdId}/apps/todo", reminder.HouseholdId, reminder.Tag), cancellationToken);
            }

            await db.SaveChangesAsync(cancellationToken);
            scanned = now;
            if (scan.Dropped > 0)
            {
                LogDropped(scan.Dropped, (int)ReminderRules.StaleAfter.TotalMinutes);
            }
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            LogFailed(exception);
        }

        _lastScan = scanned ?? _lastScan;
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Scanning reminders failed; trying again later.")]
    private partial void LogFailed(Exception exception);

    [LoggerMessage(Level = LogLevel.Information, Message = "Dropped {Count} reminders more than {Minutes} minutes late.")]
    private partial void LogDropped(int count, int minutes);
}
