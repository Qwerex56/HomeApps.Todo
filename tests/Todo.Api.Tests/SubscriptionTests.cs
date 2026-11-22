using System.Net;
using System.Net.Http.Json;
using HomeApps.Contracts.Accounts;
using HomeApps.Contracts.Households;
using Todo.Application.Subscriptions;

namespace Todo.Api.Tests;

public sealed class SubscriptionTests(TodoFactory factory) : IClassFixture<TodoFactory>
{
    [Fact]
    public async Task A_valid_token_serves_ics_anonymously_and_a_wrong_one_is_404()
    {
        var (h, token) = await SubscribedAsync();

        var ok = await factory.CreateClient().GetAsync($"/plugins/todo/feed/{token}.ics");
        var bad = await factory.CreateClient().GetAsync($"/plugins/todo/feed/{token[..^1]}x.ics");

        Assert.Equal((HttpStatusCode.OK, "text/calendar", HttpStatusCode.NotFound), (ok.StatusCode, ok.Content.Headers.ContentType?.MediaType, bad.StatusCode));
        Assert.StartsWith("BEGIN:VCALENDAR", await ok.Content.ReadAsStringAsync(), StringComparison.Ordinal);
        Assert.Single(await h.As(h.Member).GetFromJsonAsync<List<CalendarSubscriptionView>>($"{h.Api}/calendar/subscriptions", AHousehold.Json) ?? []);
    }

    [Fact]
    public async Task Leaving_the_household_makes_the_feed_404()
    {
        var (h, token) = await SubscribedAsync();
        factory.Households.SetRole(h.Id, h.Member, null);
        await factory.Harness.Bus.Publish(new MemberLeft(h.Id, h.Member, MemberLeftReason.Left));

        await Eventually(async () => (await factory.CreateClient().GetAsync($"/plugins/todo/feed/{token}.ics")).StatusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task A_disabled_account_gets_404_until_enabled_and_erase_deletes_the_link()
    {
        var (h, token) = await SubscribedAsync();
        await factory.Harness.Bus.Publish(new UserDisabled(h.Member, false));
        await Eventually(async () => await StatusAsync(token) == HttpStatusCode.NotFound);

        await factory.Harness.Bus.Publish(new UserEnabled(h.Member));
        await Eventually(async () => await StatusAsync(token) == HttpStatusCode.OK);

        await factory.Harness.Bus.Publish(new UserErased(h.Member));
        await Eventually(async () => await StatusAsync(token) == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Private_events_reach_the_feed_only_when_the_subscription_asks_for_them()
    {
        var (h, _) = await SubscribedAsync();
        var start = DateTimeOffset.UtcNow.AddHours(2);
        foreach (var (title, isPrivate) in new[] { ("Wspolne", false), ("Tajne", true) })
        {
            var created = await h.As(h.Member).PostAsJsonAsync($"{h.Api}/calendar/events", new
            {
                title, timeZoneId = "UTC", startUtc = start, endUtc = start.AddHours(1), isAllDay = false, isPrivate,
                attendees = Array.Empty<Guid>(), reminderMinutes = Array.Empty<int>(), rrule = (string?)null,
            });
            Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        }

        var plain = await CreateFeedAsync(h, "Plain", includePrivateEvents: false);
        var withPrivate = await CreateFeedAsync(h, "Private", includePrivateEvents: true);

        var plainIcs = await factory.CreateClient().GetStringAsync($"/plugins/todo/feed/{plain}.ics");
        var privateIcs = await factory.CreateClient().GetStringAsync($"/plugins/todo/feed/{withPrivate}.ics");

        Assert.Equal((true, false, true, true), (plainIcs.Contains("SUMMARY:Wspolne"), plainIcs.Contains("SUMMARY:Tajne"), privateIcs.Contains("SUMMARY:Wspolne"), privateIcs.Contains("SUMMARY:Tajne")));
    }

    [Fact]
    public async Task Revoking_removes_the_link_and_only_the_owner_may()
    {
        var (h, token) = await SubscribedAsync();
        var id = (await h.As(h.Member).GetFromJsonAsync<List<CalendarSubscriptionView>>($"{h.Api}/calendar/subscriptions", AHousehold.Json))![0].Id;

        var foreign = await h.As(h.Other).DeleteAsync($"{h.Api}/calendar/subscriptions/{id}");
        var own = await h.As(h.Member).DeleteAsync($"{h.Api}/calendar/subscriptions/{id}");

        Assert.Equal((HttpStatusCode.NotFound, HttpStatusCode.NoContent, HttpStatusCode.NotFound), (foreign.StatusCode, own.StatusCode, await StatusAsync(token)));
    }

    private static async Task<string> CreateFeedAsync(AHousehold h, string label, bool includePrivateEvents)
    {
        var response = await h.As(h.Member).PostAsJsonAsync($"{h.Api}/calendar/subscriptions", new { label, includePrivateEvents });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<CreatedSubscriptionView>(AHousehold.Json))!.Token;
    }

    private async Task<HttpStatusCode> StatusAsync(string token) => (await factory.CreateClient().GetAsync($"/plugins/todo/feed/{token}.ics")).StatusCode;

    private async Task<(AHousehold Household, string Token)> SubscribedAsync()
    {
        var h = await AHousehold.SwitchedOnAsync(factory);
        await factory.Harness.Bus.Publish(new MemberJoined(h.Id, h.Member, HouseholdRole.Member));
        await Task.Delay(300);
        var response = await h.As(h.Member).PostAsJsonAsync($"{h.Api}/calendar/subscriptions", new { label = "Phone", tasksMode = "Assigned" });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = (await response.Content.ReadFromJsonAsync<CreatedSubscriptionView>(AHousehold.Json))!;
        await Eventually(async () => await StatusAsync(created.Token) == HttpStatusCode.OK);
        return (h, created.Token);
    }

    private static async Task Eventually(Func<Task<bool>> condition)
    {
        for (var i = 0; i < 100; i++)
        {
            if (await condition())
            {
                return;
            }

            await Task.Delay(100);
        }

        Assert.Fail("The condition did not become true within 10 seconds.");
    }
}
