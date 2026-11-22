using HomeApps.BuildingBlocks.Application.Abstractions;
using HomeApps.Plugins.Notifications;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Todo.Application;
using Todo.Application.Abstractions;
using Todo.Application.Reminders;
using Todo.Domain.Common;
using Todo.Domain.Events;
using Todo.Domain.Households;
using Todo.Domain.Settings;
using Todo.Infrastructure.Jobs;
using Todo.Infrastructure.Persistence;

namespace Todo.Api.Tests;

/// <summary>No Docker needed: the scan is pure and the job runs against a fake reader and notifier.</summary>
public sealed class ReminderSchedulerTests
{
    private static readonly DateTimeOffset Now = new(2030, 3, 1, 12, 0, 0, TimeSpan.Zero);
    private static readonly Guid Household = Guid.NewGuid();

    private readonly FixedClock _time = new(Now);
    private readonly Guid _author = Guid.NewGuid();

    private CalendarEvent Event(DateTimeOffset start, int[] reminders, Guid[]? attendees = null, bool isPrivate = false) =>
        CalendarEvent.Create(Household, Actor.Member(_author), "Dentysta", null, null, "Europe/Warsaw", start, start.AddHours(1), false, isPrivate, attendees, reminders, _time).Value;

    private static HouseholdMemberProjection Member(Guid user) => new(Household, user, "Member", Now.AddDays(-10));

    private static ReminderSources Sources(IEnumerable<CalendarEvent> events, params HouseholdMemberProjection[] members) =>
        new([.. events], [], [], [], [], members, []);

    [Fact]
    public void An_event_reminder_fires_in_the_window_for_the_whole_household()
    {
        var other = Guid.NewGuid();
        var sources = Sources([Event(Now.AddMinutes(10).AddSeconds(-20), [10])], Member(_author), Member(other));

        var scan = ReminderScanner.Scan(sources, Now.AddMinutes(-1), Now);

        Assert.Equal(new[] { _author, other }.Order(), scan.Due.Select(d => d.UserId).Order());
        Assert.All(scan.Due, d => Assert.Equal(TodoPlugin.EventReminderNotification, d.Category));
    }

    [Fact]
    public void A_reminder_over_60_minutes_late_is_dropped_not_sent()
    {
        var sources = Sources([Event(Now.AddMinutes(-30), [90])], Member(_author));

        var scan = ReminderScanner.Scan(sources, Now.AddHours(-3), Now);

        Assert.Empty(scan.Due);
        Assert.Equal(1, scan.Dropped);
    }

    [Fact]
    public void Disabled_and_departed_members_get_nothing()
    {
        var gone = Member(Guid.NewGuid());
        gone.Left(Now.AddDays(-1));
        var off = Member(Guid.NewGuid());
        off.Disable();
        var sources = Sources([Event(Now.AddMinutes(5), [5])], gone, off);

        Assert.Empty(ReminderScanner.Scan(sources, Now.AddMinutes(-1), Now).Due);
    }

    [Fact]
    public void A_private_event_reminds_only_its_author()
    {
        var other = Guid.NewGuid();
        var sources = Sources([Event(Now.AddMinutes(5), [5], isPrivate: true)], Member(_author), Member(other));

        Assert.Equal(_author, Assert.Single(ReminderScanner.Scan(sources, Now.AddMinutes(-1), Now).Due).UserId);
    }

    [Fact]
    public void A_users_own_reminders_replace_the_event_ones_for_them_only()
    {
        var other = Guid.NewGuid();
        var e = Event(Now.AddMinutes(30), [30]);
        var own = new EventReminderOverride(e.Id, other, Now);
        own.Set([5], _time);
        var sources = Sources([e], Member(_author), Member(other)) with { UserOverrides = [own] };

        var scan = ReminderScanner.Scan(sources, Now.AddMinutes(-1), Now);

        Assert.Equal(_author, Assert.Single(scan.Due).UserId);
    }

    [Fact]
    public void Default_reminders_apply_when_the_event_has_none()
    {
        var settings = new PlannerSettings(Household, _author, Now);
        settings.Update([15], [], DayOfWeek.Monday, _time);
        var sources = Sources([Event(Now.AddMinutes(15), [])], Member(_author)) with { Settings = [settings] };

        Assert.Single(ReminderScanner.Scan(sources, Now.AddMinutes(-1), Now).Due);
    }

    [Fact]
    public async Task The_job_notifies_each_due_reminder_and_does_not_repeat_it_on_the_next_scan()
    {
        var time = new FixedClock(DateTimeOffset.UtcNow);
        var notifier = new RecordingNotifier();
        var reader = new FakeReader(Sources([Event(time.GetUtcNow().AddSeconds(-10), [0])], Member(_author)));
        var services = new ServiceCollection();
        services.AddSingleton<TimeProvider>(time);
        services.AddSingleton<IReminderReader>(reader);
        services.AddSingleton<INotifier>(notifier);
        services.AddScoped(_ => new TodoDbContext(new DbContextOptionsBuilder<TodoDbContext>().UseNpgsql("Host=127.0.0.1;Port=1;Timeout=1").Options, new NoPublisher()));
        var job = new CalendarReminderJob(services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>(), NullLogger<CalendarReminderJob>.Instance);

        await job.RunOnceAsync(default);
        time.Advance(TimeSpan.FromSeconds(60));
        await job.RunOnceAsync(default);

        var sent = Assert.Single(notifier.Sent);
        Assert.Equal((_author, TodoPlugin.EventReminderNotification, Household), (Assert.Single(sent.UserIds), sent.Category, sent.HouseholdId));
    }

    private sealed class FixedClock(DateTimeOffset now) : TimeProvider
    {
        private DateTimeOffset _now = now;

        public override DateTimeOffset GetUtcNow() => _now;

        public void Advance(TimeSpan by) => _now += by;
    }

    private sealed class FakeReader(ReminderSources sources) : IReminderReader
    {
        public Task<ReminderSources> LoadAsync(DateTimeOffset from, DateTimeOffset horizon, CancellationToken cancellationToken) => Task.FromResult(sources);
    }

    private sealed class RecordingNotifier : INotifier
    {
        public List<PluginNotification> Sent { get; } = [];

        public Task NotifyAsync(PluginNotification notification, CancellationToken ct)
        {
            Sent.Add(notification);
            return Task.CompletedTask;
        }
    }

    private sealed class NoPublisher : IPublisher
    {
        public Task Publish(object notification, CancellationToken ct = default) => Task.CompletedTask;

        public Task Publish<TNotification>(TNotification notification, CancellationToken ct = default)
            where TNotification : INotification => Task.CompletedTask;
    }
}
