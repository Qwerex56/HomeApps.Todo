using HomeApps.BuildingBlocks.Application.Abstractions;
using HomeApps.Contracts.Households;
using HomeApps.Plugins;
using HomeApps.Plugins.Notifications;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Todo.Application;
using Todo.Application.Abstractions;
using Todo.Application.Accounts.UserErased;
using Todo.Application.Common;
using Todo.Application.Events;
using Todo.Application.Events.GetCalendarItems;
using Todo.Application.ExternalCalendars;
using Todo.Application.Households.MemberJoined;
using Todo.Application.Households.MemberLeft;
using Todo.Application.Households.PurgeLeftLists;
using Todo.Domain.Common;
using Todo.Domain.Events;
using Todo.Domain.Households;
using Todo.Domain.Lists;
using Todo.Domain.Tasks;
using Todo.Infrastructure.Jobs;
using Todo.Infrastructure.Persistence;

namespace Todo.Api.Tests;

/// <summary>No Docker needed: handlers and the sync job run against in-memory fakes of their ports.</summary>
public sealed class ExternalCalendarHandlerTests
{
    private const string Ics = "BEGIN:VCALENDAR\r\nBEGIN:VEVENT\r\nUID:a1\r\nSUMMARY:Dentist\r\nDESCRIPTION:Floor 3\r\nLOCATION:Main St\r\nDTSTART:20300310T090000Z\r\nDTEND:20300310T100000Z\r\nEND:VEVENT\r\nEND:VCALENDAR\r\n";

    private static readonly Guid Household = Guid.NewGuid();

    private readonly MovableTime _time = new();
    private readonly FakeCalendars _calendars = new();

    private ExternalCalendarImport Import(Guid owner, ExternalCalendarVisibility visibility = ExternalCalendarVisibility.Details, Guid? household = null, bool synced = true)
    {
        var c = ExternalCalendarImport.Create(household ?? Household, owner, "https://example.com/a.ics", "Praca", null, visibility, _time).Value;
        if (synced)
        {
            c.Synced(Ics, _time.GetUtcNow());
        }

        _calendars.All.Add(c);
        return c;
    }

    // ---- GetCalendarItemsHandler

    private async Task<IReadOnlyList<CalendarItemView>> ViewAsync(Guid viewer, HouseholdRole role)
    {
        var handler = new GetCalendarItemsHandler(new EmptyReader(), _calendars, new FakeAccess(role), new FakeUser(viewer));
        var result = await handler.Handle(
            new GetCalendarItemsQuery(Household, new DateTimeOffset(2030, 3, 1, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2030, 4, 1, 0, 0, 0, TimeSpan.Zero), "external"), default);
        return result.Value;
    }

    [Fact]
    public async Task Details_are_shown_in_full_to_other_members()
    {
        Import(Guid.NewGuid(), ExternalCalendarVisibility.Details);

        var item = Assert.Single(await ViewAsync(Guid.NewGuid(), HouseholdRole.Member));

        Assert.Equal((CalendarItemType.External, "Dentist", "Floor 3", "Main St"), (item.ItemType, item.Title, item.Description, item.Location));
    }

    [Fact]
    public async Task A_busy_import_is_masked_for_others_but_not_for_its_owner()
    {
        var owner = Guid.NewGuid();
        Import(owner, ExternalCalendarVisibility.Busy);

        var other = Assert.Single(await ViewAsync(Guid.NewGuid(), HouseholdRole.Member));
        var own = Assert.Single(await ViewAsync(owner, HouseholdRole.Member));

        Assert.Equal(("Zajęty", null, null), (other.Title, other.Description, other.Location));
        Assert.Equal("Dentist", own.Title);
    }

    [Fact]
    public async Task An_only_me_import_is_hidden_from_others_and_shown_to_its_owner()
    {
        var owner = Guid.NewGuid();
        Import(owner, ExternalCalendarVisibility.OnlyMe);

        Assert.Empty(await ViewAsync(Guid.NewGuid(), HouseholdRole.Admin));
        Assert.Single(await ViewAsync(owner, HouseholdRole.Member));
    }

    [Fact]
    public async Task A_guest_sees_no_imports()
    {
        Import(Guid.NewGuid(), ExternalCalendarVisibility.Details);

        Assert.Empty(await ViewAsync(Guid.NewGuid(), HouseholdRole.Guest));
    }

    [Fact]
    public async Task An_import_whose_owner_left_is_gone_from_the_view()
    {
        var c = Import(Guid.NewGuid(), ExternalCalendarVisibility.Details);
        c.OwnerLeft(_time.GetUtcNow());

        Assert.Empty(await ViewAsync(Guid.NewGuid(), HouseholdRole.Member));
    }

    // ---- lifecycle handlers

    [Fact]
    public async Task MemberLeft_pauses_only_that_users_imports_in_that_household()
    {
        var user = Guid.NewGuid();
        var mine = Import(user);
        var elsewhere = Import(user, household: Guid.NewGuid());
        var others = Import(Guid.NewGuid());

        await new MemberLeftHandler(new NoLists(), new NoTasks(), new NoMembers(), _calendars, _time).Handle(new MemberLeftCommand(Household, user), default);

        Assert.True(mine.IsPaused);
        Assert.False(elsewhere.IsPaused);
        Assert.False(others.IsPaused);
    }

    [Fact]
    public async Task MemberJoined_brings_the_imports_back()
    {
        var user = Guid.NewGuid();
        var c = Import(user);
        c.OwnerLeft(_time.GetUtcNow());

        await new MemberJoinedHandler(new NoLists(), new NoMembers(), _calendars, _time).Handle(new MemberJoinedCommand(Household, user), default);

        Assert.False(c.IsPaused);
    }

    [Fact]
    public async Task Purge_deletes_imports_gone_for_thirty_days_and_keeps_the_rest()
    {
        var old = Import(Guid.NewGuid());
        old.OwnerLeft(_time.GetUtcNow());
        var fresh = Import(Guid.NewGuid());
        fresh.OwnerLeft(_time.GetUtcNow().AddDays(25));
        var present = Import(Guid.NewGuid());
        _time.Advance(TimeSpan.FromDays(30));

        var result = await new PurgeLeftListsHandler(new NoLists(), _calendars, _time).Handle(new PurgeLeftListsCommand(), default);

        Assert.Equal(1, result.Value);
        Assert.Equal([fresh, present], _calendars.All);
    }

    [Fact]
    public async Task UserErased_deletes_the_imports_in_every_household()
    {
        var user = Guid.NewGuid();
        Import(user);
        Import(user, household: Guid.NewGuid());
        var kept = Import(Guid.NewGuid());

        await new UserErasedHandler(new NoSubscriptions(), _calendars, new NoMembers()).Handle(new UserErasedCommand(user), default);

        Assert.Equal([kept], _calendars.All);
    }

    // ---- ExternalCalendarSyncJob

    private sealed class Rig
    {
        public required ExternalCalendarSyncJob Job { get; init; }

        public required FakeSsrfSafeClient Client { get; init; }

        public required RecordingNotifier Notifier { get; init; }
    }

    private Rig JobRig()
    {
        var client = new FakeSsrfSafeClient();
        var notifier = new RecordingNotifier();
        var services = new ServiceCollection();
        services.AddSingleton<TimeProvider>(_time);
        services.AddSingleton<IExternalCalendarRepository>(_calendars);
        services.AddSingleton<ISsrfSafeClient>(client);
        services.AddSingleton<INotifier>(notifier);
        services.AddScoped<ExternalCalendarSyncer>();
        // Nothing is tracked, so SaveChangesAsync has no work and never opens this (unreachable) connection.
        services.AddScoped(_ => new TodoDbContext(new DbContextOptionsBuilder<TodoDbContext>().UseNpgsql("Host=127.0.0.1;Port=1;Timeout=1").Options, new NoPublisher()));
        var provider = services.BuildServiceProvider();
        return new Rig { Job = new ExternalCalendarSyncJob(provider.GetRequiredService<IServiceScopeFactory>(), NullLogger<ExternalCalendarSyncJob>.Instance), Client = client, Notifier = notifier };
    }

    [Fact]
    public async Task The_job_skips_imports_whose_owner_left_and_syncs_the_others()
    {
        var rig = JobRig();
        var paused = Import(Guid.NewGuid(), synced: false);
        paused.OwnerLeft(_time.GetUtcNow());
        var active = Import(Guid.NewGuid(), synced: false);
        rig.Client.Answer(FluentResults.Result.Ok(Ics));

        await rig.Job.RunOnceAsync(default);

        Assert.Single(rig.Client.Requested);
        Assert.Null(paused.LastSyncedAt);
        Assert.NotNull(active.LastSyncedAt);
    }

    [Fact]
    public async Task The_job_leaves_imports_tried_within_half_an_hour_alone()
    {
        var rig = JobRig();
        Import(Guid.NewGuid());

        await rig.Job.RunOnceAsync(default);

        Assert.Empty(rig.Client.Requested);
    }

    [Fact]
    public async Task The_owner_is_notified_once_when_the_failure_streak_reaches_144()
    {
        var rig = JobRig();
        var owner = Guid.NewGuid();
        var c = Import(owner, synced: false);
        for (var i = 0; i < ExternalCalendarImport.NotifyAfterFailures - 2; i++)
        {
            c.SyncFailed("boom", _time.GetUtcNow());
        }

        for (var round = 0; round < 3; round++)
        {
            _time.Advance(ExternalCalendarImport.SyncInterval);
            rig.Client.Answer(FluentResults.Result.Fail<string>(PlannerErrors.SyncFailed));
            await rig.Job.RunOnceAsync(default);
        }

        Assert.Equal(ExternalCalendarImport.NotifyAfterFailures + 1, c.SyncFailureCount);
        var sent = Assert.Single(rig.Notifier.Sent);
        Assert.Equal((owner, TodoPlugin.CalendarSyncNotification, Household), (Assert.Single(sent.UserIds), sent.Category, sent.HouseholdId));
    }

    // ---- fakes

    private sealed class FakeCalendars : IExternalCalendarRepository
    {
        public List<ExternalCalendarImport> All { get; } = [];

        public Task<ExternalCalendarImport?> GetAsync(Guid householdId, Guid id, CancellationToken ct) =>
            Task.FromResult(All.FirstOrDefault(c => c.Id == id && c.HouseholdId == householdId));

        public Task<IReadOnlyList<ExternalCalendarImport>> ListOfUserAsync(Guid householdId, Guid userId, CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<ExternalCalendarImport>>(All.Where(c => c.HouseholdId == householdId && c.UserId == userId && !c.IsPaused).ToList());

        public Task<IReadOnlyList<ExternalCalendarImport>> ListAllOfUserAsync(Guid? householdId, Guid userId, CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<ExternalCalendarImport>>(All.Where(c => c.UserId == userId && (householdId is null || c.HouseholdId == householdId)).ToList());

        // Mirrors ExternalCalendarRepository.ListVisibleAsync.
        public Task<IReadOnlyList<ExternalCalendarImport>> ListVisibleAsync(Guid householdId, Guid viewerId, CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<ExternalCalendarImport>>(All
                .Where(c => c.HouseholdId == householdId && !c.IsPaused && (c.UserId == viewerId || c.Visibility != ExternalCalendarVisibility.OnlyMe)).ToList());

        public Task<IReadOnlyList<ExternalCalendarImport>> ListDueForSyncAsync(DateTimeOffset now, CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<ExternalCalendarImport>>(All.Where(c => c.IsDueForSync(now)).ToList());

        public Task<IReadOnlyList<ExternalCalendarImport>> ListDueForPurgeAsync(DateTimeOffset now, CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<ExternalCalendarImport>>(All.Where(c => c.IsDueForPurge(now)).ToList());

        public void Add(ExternalCalendarImport import) => All.Add(import);

        public void Remove(ExternalCalendarImport import) => All.Remove(import);
    }

    private sealed class FakeAccess(HouseholdRole role) : IHouseholdAccess
    {
        public Task<HouseholdMembership?> GetMembershipAsync(Guid householdId, CancellationToken ct) =>
            Task.FromResult<HouseholdMembership?>(new HouseholdMembership(role, DateTimeOffset.UnixEpoch, null));
    }

    private sealed class FakeUser(Guid id) : ICurrentUser
    {
        public bool IsAuthenticated => true;

        public Guid? UserId => id;

        public string? Email => null;

        public DateTimeOffset? AuthenticatedAt => null;

        public IReadOnlyCollection<string> Roles => [];

        public bool IsInRole(string role) => false;

        public Guid RequiredUserId => id;
    }

    private sealed class EmptyReader : ICalendarReader
    {
        public Task<IReadOnlyList<CalendarEvent>> EventsAsync(Guid h, Guid u, bool g, DateTimeOffset f, DateTimeOffset t, CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<CalendarEvent>>([]);

        public Task<IReadOnlyList<(EventSeries Series, IReadOnlyList<EventOverride> Overrides)>> SeriesAsync(Guid h, Guid u, bool g, DateTimeOffset f, DateTimeOffset t, CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<(EventSeries, IReadOnlyList<EventOverride>)>>([]);

        public Task<IReadOnlyList<TaskView>> TasksDueAsync(Guid h, Guid u, DateOnly f, DateOnly l, CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<TaskView>>([]);
    }

    private sealed class NoLists : ITodoListRepository
    {
        public Task<TodoList?> GetAsync(Guid householdId, Guid listId, CancellationToken ct) => Task.FromResult<TodoList?>(null);

        public Task<IReadOnlyList<TodoList>> ListPrivateOfAsync(Guid householdId, Guid ownerId, CancellationToken ct) => Task.FromResult<IReadOnlyList<TodoList>>([]);

        public Task<IReadOnlyList<TodoList>> ListDueForPurgeAsync(DateTimeOffset now, CancellationToken ct) => Task.FromResult<IReadOnlyList<TodoList>>([]);

        public void Add(TodoList list)
        {
        }

        public void Remove(TodoList list)
        {
        }
    }

    private sealed class NoTasks : ITodoTaskRepository
    {
        public Task<TodoTask?> GetAsync(Guid householdId, Guid taskId, CancellationToken ct) => Task.FromResult<TodoTask?>(null);

        public Task<IReadOnlyList<TodoTask>> ListAssignedToAsync(Guid householdId, Guid userId, CancellationToken ct) => Task.FromResult<IReadOnlyList<TodoTask>>([]);

        public Task<decimal?> LastPositionAsync(Guid listId, Guid? exceptTaskId, CancellationToken ct) => Task.FromResult<decimal?>(null);

        public Task<decimal?> PositionBeforeAsync(Guid listId, decimal position, Guid exceptTaskId, CancellationToken ct) => Task.FromResult<decimal?>(null);

        public Task<bool> HasOpenInSeriesAsync(Guid householdId, Guid seriesId, Guid exceptTaskId, CancellationToken ct) => Task.FromResult(false);

        public void Add(TodoTask task)
        {
        }

        public void Remove(TodoTask task)
        {
        }
    }

    private sealed class NoMembers : IInactiveMarkerRepository
    {
        public Task MarkAsync(Guid? householdId, Guid? userId, CancellationToken ct) => Task.CompletedTask;

        public Task ClearAsync(Guid? householdId, Guid? userId, CancellationToken ct) => Task.CompletedTask;

        public Task<bool> IsInactiveAsync(Guid householdId, Guid userId, CancellationToken ct) => Task.FromResult(false);
    }

    private sealed class NoSubscriptions : ICalendarSubscriptionRepository
    {
        public Task<CalendarSubscription?> GetAsync(Guid householdId, Guid userId, Guid id, CancellationToken ct) => Task.FromResult<CalendarSubscription?>(null);

        public Task<IReadOnlyList<CalendarSubscription>> ListAsync(Guid householdId, Guid userId, CancellationToken ct) => Task.FromResult<IReadOnlyList<CalendarSubscription>>([]);

        public Task<IReadOnlyList<CalendarSubscription>> ListOfUserAsync(Guid userId, CancellationToken ct) => Task.FromResult<IReadOnlyList<CalendarSubscription>>([]);

        public Task<CalendarSubscription?> FindByTokenHashAsync(string tokenHash, CancellationToken ct) => Task.FromResult<CalendarSubscription?>(null);

        public Task TouchAsync(Guid id, DateTimeOffset now, CancellationToken ct) => Task.CompletedTask;

        public void Add(CalendarSubscription subscription)
        {
        }

        public void Remove(CalendarSubscription subscription)
        {
        }
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
