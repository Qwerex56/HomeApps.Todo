using System.Net;
using System.Net.Http.Json;
using Todo.Application.Settings;

namespace Todo.Api.Tests;

/// <summary>Needs Docker (Testcontainers Postgres).</summary>
public sealed class PlannerSettingsTests(TodoFactory factory) : IClassFixture<TodoFactory>
{
    private static readonly int[] Ten = [10];
    private static readonly int[] Hour = [60];

    [Fact]
    public async Task Settings_default_then_save_per_user_and_reject_a_bad_zone()
    {
        var h = await AHousehold.SwitchedOnAsync(factory);
        var url = $"{h.Api}/calendar/settings";

        var first = await h.As(h.Member).GetFromJsonAsync<PlannerSettingsView>(url, AHousehold.Json);
        var bad = await h.As(h.Member).PutAsJsonAsync(url, new { eventReminders = Ten, firstDayOfWeek = 1, timeZone = "Mars/Base" });
        var ok = await h.As(h.Member).PutAsJsonAsync(url, new { eventReminders = Ten, taskReminders = Hour, firstDayOfWeek = 0, timeZone = "Europe/Warsaw" });
        var other = await h.As(h.Owner).GetFromJsonAsync<PlannerSettingsView>(url, AHousehold.Json);

        Assert.Equal((null, HttpStatusCode.BadRequest, HttpStatusCode.OK, null), (first!.TimeZone, bad.StatusCode, ok.StatusCode, other!.TimeZone));
        Assert.Equal("Europe/Warsaw", (await ok.Content.ReadFromJsonAsync<PlannerSettingsView>(AHousehold.Json))!.TimeZone);
    }

    [Fact]
    public async Task A_guest_reads_and_saves_their_own_settings()
    {
        var h = await AHousehold.SwitchedOnAsync(factory);
        var url = $"{h.Api}/calendar/settings";

        var get = await h.As(h.Guest).GetAsync(url);
        var put = await h.As(h.Guest).PutAsJsonAsync(url, new { eventReminders = Ten, taskReminders = Hour, firstDayOfWeek = 1, timeZone = "Europe/Warsaw" });

        Assert.Equal((HttpStatusCode.OK, HttpStatusCode.OK), (get.StatusCode, put.StatusCode));
    }

    [Fact]
    public async Task An_unknown_household_is_404_household_not_found()
    {
        var h = await AHousehold.SwitchedOnAsync(factory);
        var url = $"/plugins/todo/api/v1/households/{Guid.NewGuid()}/calendar/settings";

        var get = await h.As(h.Member).GetAsync(url);

        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
        Assert.Equal("household.not_found", await AHousehold.CodeOf(get));
    }
}
