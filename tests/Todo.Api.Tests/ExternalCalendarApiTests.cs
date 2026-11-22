using System.Net;
using System.Net.Http.Json;
using HomeApps.Contracts.Households;
using System.Text.Json;
using FluentResults;
using Todo.Application.ExternalCalendars;
using Todo.Domain.Common;

namespace Todo.Api.Tests;

/// <summary>Docker needed (Testcontainers Postgres). Downloads are answered by <see cref="TodoFactory.Ssrf"/>; refusals use the real client.</summary>
public sealed class ExternalCalendarApiTests(TodoFactory factory) : IClassFixture<TodoFactory>
{
    [Fact]
    public async Task A_new_household_has_no_imported_calendars()
    {
        var h = await AHousehold.SwitchedOnAsync(factory);

        var list = await h.As(h.Member).GetFromJsonAsync<List<ExternalCalendarView>>($"{h.Api}/calendar/external", AHousehold.Json);

        Assert.Empty(list ?? []);
    }

    [Theory]
    [InlineData("http://127.0.0.1/a.ics", "planner.ssrf_rejected")]
    [InlineData("http://localhost:5400/a.ics", "planner.ssrf_rejected")]
    [InlineData("http://169.254.169.254/latest/meta-data", "planner.ssrf_rejected")]
    [InlineData("ftp://example.com/a.ics", "planner.invalid_url")]
    [InlineData("https://user:secret@example.com/a.ics", "planner.invalid_url")]
    public async Task Importing_an_internal_or_malformed_address_is_a_400_with_its_code(string url, string code)
    {
        var h = await AHousehold.SwitchedOnAsync(factory);

        var response = await h.As(h.Member).PostAsJsonAsync($"{h.Api}/calendar/external", new { url, name = "Praca", color = (string?)null, visibility = "Details" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains(code, await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Syncing_or_removing_an_unknown_import_is_404()
    {
        var h = await AHousehold.SwitchedOnAsync(factory);
        var id = Guid.NewGuid();

        var sync = await h.As(h.Member).PostAsync($"{h.Api}/calendar/external/{id}/sync", null);
        var remove = await h.As(h.Member).DeleteAsync($"{h.Api}/calendar/external/{id}");

        Assert.Equal((HttpStatusCode.NotFound, HttpStatusCode.NotFound), (sync.StatusCode, remove.StatusCode));
    }

    private const string Ics = "BEGIN:VCALENDAR\r\nBEGIN:VEVENT\r\nUID:a1\r\nSUMMARY:Dentist\r\nDTSTART:20300310T090000Z\r\nDTEND:20300310T100000Z\r\nEND:VEVENT\r\nEND:VCALENDAR\r\n";

    private const string Secret = "https://example.com/secret-token.ics";

    private async Task<(AHousehold H, Guid Id)> ImportedAsync(string visibility = "Details")
    {
        var h = await AHousehold.SwitchedOnAsync(factory);
        factory.Ssrf.Answer(Result.Ok(Ics));
        var response = await h.As(h.Member).PostAsJsonAsync($"{h.Api}/calendar/external", new { url = Secret, name = "Praca", color = (string?)null, visibility });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (h, (await response.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetGuid());
    }

    [Fact]
    public async Task Importing_creates_a_calendar_and_the_url_is_never_returned()
    {
        var (h, id) = await ImportedAsync();

        var body = await h.As(h.Member).GetStringAsync($"{h.Api}/calendar/external");

        Assert.Contains(id.ToString(), body);
        Assert.DoesNotContain("secret-token", body);
        Assert.DoesNotContain("example.com", body);
    }

    [Fact]
    public async Task Importing_something_that_is_not_a_calendar_is_a_400_parse_error()
    {
        var h = await AHousehold.SwitchedOnAsync(factory);
        factory.Ssrf.Answer(Result.Ok("<html>nope</html>"));

        var response = await h.As(h.Member).PostAsJsonAsync($"{h.Api}/calendar/external", new { url = Secret, name = "Praca", visibility = "Busy" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("planner.parse_error", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Sync_refreshes_and_a_failed_download_is_a_400_sync_failed()
    {
        var (h, id) = await ImportedAsync();

        factory.Ssrf.Answer(Result.Ok(Ics));
        var ok = await h.As(h.Member).PostAsync($"{h.Api}/calendar/external/{id}/sync", null);
        factory.Ssrf.Answer(Result.Fail<string>(PlannerErrors.SyncFailed));
        var failed = await h.As(h.Member).PostAsync($"{h.Api}/calendar/external/{id}/sync", null);

        Assert.Equal(HttpStatusCode.OK, ok.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, failed.StatusCode);
        Assert.Contains("planner.sync_failed", await failed.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Only_the_owner_removes_an_import_and_others_cannot_sync_it()
    {
        var (h, id) = await ImportedAsync();

        var foreign = await h.As(h.Other).DeleteAsync($"{h.Api}/calendar/external/{id}");
        var foreignSync = await h.As(h.Other).PostAsync($"{h.Api}/calendar/external/{id}/sync", null);
        var own = await h.As(h.Member).DeleteAsync($"{h.Api}/calendar/external/{id}");

        Assert.Equal((HttpStatusCode.Forbidden, HttpStatusCode.NotFound, HttpStatusCode.NoContent), (foreign.StatusCode, foreignSync.StatusCode, own.StatusCode));
        Assert.Contains("planner.not_owner", await foreign.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task A_guest_cannot_use_external_calendars()
    {
        var h = await AHousehold.SwitchedOnAsync(factory);

        var list = await h.As(h.Guest).GetAsync($"{h.Api}/calendar/external");
        var add = await h.As(h.Guest).PostAsJsonAsync($"{h.Api}/calendar/external", new { url = Secret, name = "x", visibility = "Busy" });

        Assert.Equal((HttpStatusCode.Forbidden, HttpStatusCode.Forbidden), (list.StatusCode, add.StatusCode));
        Assert.Contains("plugin.forbidden", await add.Content.ReadAsStringAsync());
    }

    [Theory]
    [InlineData("OnlyMe", 0, false)]
    [InlineData("Busy", 1, false)]
    [InlineData("Details", 1, true)]
    public async Task Others_see_an_import_as_its_visibility_allows(string visibility, int count, bool details)
    {
        var (h, _) = await ImportedAsync(visibility);

        var window = await h.As(h.Other).GetFromJsonAsync<JsonElement>($"{h.Api}/calendar/events?from=2030-03-01T00:00:00Z&to=2030-04-01T00:00:00Z&layers=external");
        var guest = await h.As(h.Guest).GetFromJsonAsync<JsonElement>($"{h.Api}/calendar/events?from=2030-03-01T00:00:00Z&to=2030-04-01T00:00:00Z&layers=external");

        Assert.Equal(count, window.GetArrayLength());
        Assert.Equal(0, guest.GetArrayLength());
        if (count > 0)
        {
            Assert.Equal(details ? "Dentist" : "Zajęty", window[0].GetProperty("title").GetString());
        }
    }
}
