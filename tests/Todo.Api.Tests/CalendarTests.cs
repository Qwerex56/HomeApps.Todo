using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Todo.Api.Tests;

/// <summary>Calendar events, series and instance scopes over HTTP (needs Docker: Testcontainers Postgres).</summary>
public sealed class CalendarTests(TodoFactory factory) : IClassFixture<TodoFactory>
{
    private static readonly DateTimeOffset Start = new(2030, 3, 4, 10, 0, 0, TimeSpan.Zero);

    private static readonly int[] Ten = [10];

    private static object Event(string title, object? more = null, string? rrule = null, bool isPrivate = false, Guid[]? attendees = null) => new
    {
        title, timeZoneId = "UTC", startUtc = Start, endUtc = Start.AddHours(1), isAllDay = false, isPrivate, attendees = attendees ?? [], reminderMinutes = Array.Empty<int>(), rrule,
    };

    private static async Task<Guid> IdOf(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetGuid();
    }

    private static Task<HttpResponseMessage> Create(AHousehold h, Guid user, object body) =>
        h.As(user).PostAsJsonAsync($"{h.Api}/calendar/events", body);

    private static Task<HttpResponseMessage> Instance(AHousehold h, Guid user, Guid id, string date, object body) =>
        h.As(user).PutAsJsonAsync($"{h.Api}/calendar/events/{id}/instances/{date}", body);

    private static async Task<JsonElement> Window(AHousehold h, Guid user, string layers = "events,private,tasks") =>
        await h.As(user).GetFromJsonAsync<JsonElement>($"{h.Api}/calendar/events?from=2030-03-01T00:00:00Z&to=2030-04-01T00:00:00Z&layers={layers}");

    [Fact]
    public async Task An_event_is_created_updated_and_deleted()
    {
        var h = await AHousehold.SwitchedOnAsync(factory);
        var id = await IdOf(await Create(h, h.Member, Event("Obiad")));

        var updated = await h.As(h.Member).PutAsJsonAsync($"{h.Api}/calendar/events/{id}", Event("Kolacja"));
        var deleted = await h.As(h.Member).DeleteAsync($"{h.Api}/calendar/events/{id}");
        var gone = await h.As(h.Member).GetAsync($"{h.Api}/calendar/events/{id}");

        Assert.Equal((HttpStatusCode.OK, HttpStatusCode.NoContent, HttpStatusCode.NotFound), (updated.StatusCode, deleted.StatusCode, gone.StatusCode));
    }

    [Fact]
    public async Task Only_attendees_edit_and_only_the_author_or_an_admin_deletes()
    {
        var h = await AHousehold.SwitchedOnAsync(factory);
        var id = await IdOf(await Create(h, h.Member, Event("Lekarz", attendees: [h.Member])));

        var edit = await h.As(h.Other).PutAsJsonAsync($"{h.Api}/calendar/events/{id}", Event("Zmiana", attendees: [h.Member]));
        var delete = await h.As(h.Other).DeleteAsync($"{h.Api}/calendar/events/{id}");

        Assert.Equal((HttpStatusCode.Forbidden, "planner.cannot_edit_event"), (edit.StatusCode, await AHousehold.CodeOf(edit)));
        Assert.Equal((HttpStatusCode.Forbidden, "planner.cannot_delete_event"), (delete.StatusCode, await AHousehold.CodeOf(delete)));
    }

    [Fact]
    public async Task Guests_read_shared_events_but_not_private_series_and_cannot_write()
    {
        var h = await AHousehold.SwitchedOnAsync(factory);
        await IdOf(await Create(h, h.Member, Event("Wspólne")));
        await IdOf(await Create(h, h.Member, Event("Prywatna seria", rrule: "FREQ=DAILY;COUNT=2", isPrivate: true)));

        var seen = await Window(h, h.Guest);
        var write = await Create(h, h.Guest, Event("Nie"));

        Assert.Equal(["Wspólne"], seen.EnumerateArray().Select(i => i.GetProperty("title").GetString()));
        Assert.Equal((HttpStatusCode.Forbidden, "plugin.forbidden"), (write.StatusCode, await AHousehold.CodeOf(write)));
    }

    [Fact]
    public async Task Layers_filter_events_and_private_items()
    {
        var h = await AHousehold.SwitchedOnAsync(factory);
        await IdOf(await Create(h, h.Member, Event("Wspólne")));
        await IdOf(await Create(h, h.Member, Event("Moje", isPrivate: true)));

        var events = await Window(h, h.Member, "events");
        var priv = await Window(h, h.Member, "private");

        Assert.Equal(["Wspólne"], events.EnumerateArray().Select(i => i.GetProperty("title").GetString()));
        Assert.Equal(["Moje"], priv.EnumerateArray().Select(i => i.GetProperty("title").GetString()));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("Nonsense")]
    public async Task A_missing_or_invalid_scope_is_400(string? scope)
    {
        var h = await AHousehold.SwitchedOnAsync(factory);
        var id = await IdOf(await Create(h, h.Member, Event("Seria", rrule: "FREQ=DAILY;COUNT=5")));

        var response = await Instance(h, h.Member, id, "2030-03-05", new { scope, title = "X" });

        Assert.Equal((HttpStatusCode.BadRequest, "planner.invalid_scope"), (response.StatusCode, await AHousehold.CodeOf(response)));
    }

    [Fact]
    public async Task ThisInstance_changes_only_that_day_and_keeps_earlier_changes_on_null_fields()
    {
        var h = await AHousehold.SwitchedOnAsync(factory);
        var id = await IdOf(await Create(h, h.Member, Event("Seria", rrule: "FREQ=DAILY;COUNT=3")));

        (await Instance(h, h.Member, id, "2030-03-05", new { scope = "ThisInstance", title = "Inny" })).EnsureSuccessStatusCode();
        (await Instance(h, h.Member, id, "2030-03-05", new { scope = "ThisInstance", reminderMinutes = Ten })).EnsureSuccessStatusCode();

        var titles = (await Window(h, h.Member)).EnumerateArray().Select(i => i.GetProperty("title").GetString());
        Assert.Equal(["Seria", "Inny", "Seria"], titles);
    }

    [Fact]
    public async Task Deleting_one_instance_cancels_it_and_it_cannot_be_modified_afterwards()
    {
        var h = await AHousehold.SwitchedOnAsync(factory);
        var id = await IdOf(await Create(h, h.Member, Event("Seria", rrule: "FREQ=DAILY;COUNT=3")));

        var deleted = await h.As(h.Member).DeleteAsync($"{h.Api}/calendar/events/{id}/instances/2030-03-05?scope=ThisInstance");
        var modify = await Instance(h, h.Member, id, "2030-03-05", new { scope = "ThisInstance", title = "Wracam" });

        Assert.Equal((HttpStatusCode.NoContent, HttpStatusCode.NotFound), (deleted.StatusCode, modify.StatusCode));
        Assert.Equal("planner.series_not_found", await AHousehold.CodeOf(modify));
        Assert.Equal(2, (await Window(h, h.Member)).GetArrayLength());
    }

    [Fact]
    public async Task ThisAndFuture_splits_the_series_and_All_drops_moved_occurrences()
    {
        var h = await AHousehold.SwitchedOnAsync(factory);
        var split = await IdOf(await Create(h, h.Member, Event("Seria", rrule: "FREQ=DAILY;COUNT=5")));
        var all = await IdOf(await Create(h, h.Member, Event("Druga", rrule: "FREQ=DAILY;COUNT=3")));

        (await Instance(h, h.Member, split, "2030-03-06", new { scope = "ThisAndFuture", title = "Od teraz" })).EnsureSuccessStatusCode();
        (await Instance(h, h.Member, all, "2030-03-05", new { scope = "ThisInstance", startUtc = Start.AddDays(1).AddHours(2), endUtc = Start.AddDays(1).AddHours(3) })).EnsureSuccessStatusCode();
        (await Instance(h, h.Member, all, "2030-03-04", new { scope = "All", startUtc = Start.AddHours(1), endUtc = Start.AddHours(2) })).EnsureSuccessStatusCode();

        var items = (await Window(h, h.Member)).EnumerateArray().ToList();
        Assert.Equal(2, items.Count(i => i.GetProperty("title").GetString() == "Seria"));
        Assert.Contains(items, i => i.GetProperty("title").GetString() == "Od teraz");
        Assert.Equal(3, items.Count(i => i.GetProperty("title").GetString() == "Druga"));
    }

    [Fact]
    public async Task An_unknown_series_is_404_and_a_malformed_date_does_not_route()
    {
        var h = await AHousehold.SwitchedOnAsync(factory);

        var unknown = await Instance(h, h.Member, Guid.NewGuid(), "2030-03-05", new { scope = "All" });
        var malformed = await Instance(h, h.Member, Guid.NewGuid(), "5-3-2030", new { scope = "All" });

        Assert.Equal((HttpStatusCode.NotFound, "planner.series_not_found"), (unknown.StatusCode, await AHousehold.CodeOf(unknown)));
        Assert.Equal(HttpStatusCode.NotFound, malformed.StatusCode);
    }

    [Fact]
    public async Task Tasks_due_on_the_exclusive_end_day_are_not_in_the_window()
    {
        var h = await AHousehold.SwitchedOnAsync(factory);
        var home = await h.HomeListAsync();
        await h.AddTaskAsync(h.Member, home.Id, "W oknie", new { due = new { date = "2030-03-31" } });
        await h.AddTaskAsync(h.Member, home.Id, "Poza oknem", new { due = new { date = "2030-04-01" } });

        var items = await Window(h, h.Member, "tasks");

        Assert.Equal(["W oknie"], items.EnumerateArray().Select(i => i.GetProperty("title").GetString()));
    }
}
