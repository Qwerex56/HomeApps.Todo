using System.Net;
using System.Net.Http.Json;
using HomeApps.Contracts.Households;
using Todo.Application.ExternalCalendars;

namespace Todo.Api.Tests;

/// <summary>Docker needed (Testcontainers Postgres). Only refusals and lookups: a real download would need an external host.</summary>
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
}
