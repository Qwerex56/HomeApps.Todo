using System.Net;
using System.Net.Http.Json;
using Todo.Application.Settings;

namespace Todo.Api.Tests;

/// <summary>Needs Docker (Testcontainers Postgres).</summary>
public sealed class PlannerSettingsTests(TodoFactory factory) : IClassFixture<TodoFactory>
{
    [Fact]
    public async Task Settings_default_then_save_per_user_and_reject_a_bad_zone()
    {
        var h = await AHousehold.EnabledAsync(factory);
        var url = $"{h.Api}/calendar/settings";

        var first = await h.As(h.Member).GetFromJsonAsync<PlannerSettingsView>(url, AHousehold.Json);
        var bad = await h.As(h.Member).PutAsJsonAsync(url, new { eventReminders = new[] { 10 }, firstDayOfWeek = 1, timeZone = "Mars/Base" });
        var ok = await h.As(h.Member).PutAsJsonAsync(url, new { eventReminders = new[] { 10 }, taskReminders = new[] { 60 }, firstDayOfWeek = 0, timeZone = "Europe/Warsaw" });
        var other = await h.As(h.Owner).GetFromJsonAsync<PlannerSettingsView>(url, AHousehold.Json);

        Assert.Equal((null, HttpStatusCode.BadRequest, HttpStatusCode.OK, null), (first!.TimeZone, bad.StatusCode, ok.StatusCode, other!.TimeZone));
        Assert.Equal("Europe/Warsaw", (await ok.Content.ReadFromJsonAsync<PlannerSettingsView>(AHousehold.Json))!.TimeZone);
    }
}
