using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using HomeApps.Contracts.Households;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Todo.Application.Households;
using Todo.Domain.Lists;
using Todo.Domain.Tasks;
using Contracts = HomeApps.Todo.Contracts;

namespace Todo.Api.Tests;

public sealed class TodoTests(TodoFactory factory) : IClassFixture<TodoFactory>
{
    [Fact]
    public async Task The_manifest_declares_permissions_and_events()
    {
        var json = await factory.CreateClient().GetFromJsonAsync<JsonNode>("/plugins/todo/manifest");

        Assert.Equal("todo", json!["id"]!.GetValue<string>());
        Assert.Equal("1.0.0", json["version"]!.GetValue<string>());
        Assert.Equal(["tasks.read", "tasks.write", "tasks.manage"], json["permissions"]!.AsArray().Select(p => p!["id"]!.GetValue<string>()));
        Assert.Equal("HomeApps.Todo.Contracts.TaskCompleted", json["events"]!["publishes"]![0]!.GetValue<string>());
    }

    [Fact]
    public async Task The_first_switch_on_creates_the_shared_list_Dom()
    {
        var h = await AHousehold.SwitchedOnAsync(factory);

        var lists = await h.ListsAsync(h.Guest);

        var home = Assert.Single(lists);
        Assert.Equal(("Dom", ListVisibility.Shared, false, 0), (home.Name, home.Visibility, home.Archived, home.OpenTasks));
    }

    [Fact]
    public async Task Guests_read_but_do_not_write()
    {
        var h = await AHousehold.SwitchedOnAsync(factory);
        var home = await h.HomeListAsync();
        await h.AddTaskAsync(h.Member, home.Id, "Kupić mleko");

        var tasks = await h.TasksAsync(h.Guest);
        var byGuest = await h.PostTaskAsync(h.Guest, home.Id, "Też coś");

        Assert.Equal("Kupić mleko", Assert.Single(tasks).Title);
        Assert.Equal(HttpStatusCode.Forbidden, byGuest.StatusCode);
        Assert.Equal("plugin.forbidden", await AHousehold.CodeOf(byGuest));
    }

    [Fact]
    public async Task Private_lists_are_seen_only_by_their_owner_not_even_by_the_Owner_of_the_household()
    {
        var h = await AHousehold.SwitchedOnAsync(factory);
        var mine = await h.CreateListAsync(h.Member, "Moje", "Private");
        await h.AddTaskAsync(h.Member, mine, "Prezent dla mamy");

        var ownerLists = await h.ListsAsync(h.Owner);
        var ownerTasks = await h.TasksAsync(h.Owner);
        var ownerAsks = await h.As(h.Owner).GetAsync($"{h.Api}/tasks?list={mine}");
        var ownerAdds = await h.PostTaskAsync(h.Owner, mine, "Podglądam");

        Assert.DoesNotContain(ownerLists, l => l.Id == mine);
        Assert.Empty(ownerTasks);
        Assert.Equal(HttpStatusCode.NotFound, ownerAsks.StatusCode);
        Assert.Equal("list.not_found", await AHousehold.CodeOf(ownerAsks));
        Assert.Equal("list.not_found", await AHousehold.CodeOf(ownerAdds));
        Assert.Contains(await h.ListsAsync(h.Member), l => l.Id == mine && l.OpenTasks == 1);
    }

    [Fact]
    public async Task Shared_lists_are_created_and_managed_by_admins_only()
    {
        var h = await AHousehold.SwitchedOnAsync(factory);
        var home = await h.HomeListAsync();

        var memberCreates = await h.As(h.Member).PostAsJsonAsync($"{h.Api}/lists", new { name = "Zakupy", visibility = "Shared" });
        var memberRenames = await h.As(h.Member).PutAsJsonAsync($"{h.Api}/lists/{home.Id}", new { name = "Mój dom" });
        var adminRenames = await h.As(h.Admin).PutAsJsonAsync($"{h.Api}/lists/{home.Id}", new { name = "Nasz dom" });

        Assert.Equal(HttpStatusCode.Forbidden, memberCreates.StatusCode);
        Assert.Equal("list.forbidden", await AHousehold.CodeOf(memberCreates));
        Assert.Equal("list.forbidden", await AHousehold.CodeOf(memberRenames));
        Assert.Equal(HttpStatusCode.NoContent, adminRenames.StatusCode);
        Assert.Equal("Nasz dom", (await h.ListsAsync(h.Guest)).Single().Name);
    }

    [Fact]
    public async Task An_archived_list_is_read_only_until_restored()
    {
        var h = await AHousehold.SwitchedOnAsync(factory);
        var home = await h.HomeListAsync();

        (await h.As(h.Admin).PostAsync(new Uri($"{h.Api}/lists/{home.Id}/archive", UriKind.Relative), null)).EnsureSuccessStatusCode();
        var whileArchived = await h.PostTaskAsync(h.Member, home.Id, "X");
        (await h.As(h.Admin).PostAsync(new Uri($"{h.Api}/lists/{home.Id}/restore", UriKind.Relative), null)).EnsureSuccessStatusCode();
        var afterwards = await h.PostTaskAsync(h.Member, home.Id, "X");

        Assert.Equal(HttpStatusCode.Conflict, whileArchived.StatusCode);
        Assert.Equal("list.archived", await AHousehold.CodeOf(whileArchived));
        Assert.Equal(HttpStatusCode.Created, afterwards.StatusCode);
    }

    [Fact]
    public async Task Ticking_off_a_shared_task_is_published_a_private_one_is_not()
    {
        var h = await AHousehold.SwitchedOnAsync(factory);
        var home = await h.HomeListAsync();
        var mine = await h.CreateListAsync(h.Member, "Moje", "Private");
        var shared = await h.AddTaskAsync(h.Member, home.Id, "Wynieść śmieci");
        var secret = await h.AddTaskAsync(h.Member, mine, "Sekret");

        (await h.As(h.Other).PostAsync(new Uri($"{h.Api}/tasks/{shared}/complete", UriKind.Relative), null)).EnsureSuccessStatusCode();
        (await h.As(h.Other).PostAsync(new Uri($"{h.Api}/tasks/{shared}/complete", UriKind.Relative), null)).EnsureSuccessStatusCode();
        (await h.As(h.Member).PostAsync(new Uri($"{h.Api}/tasks/{secret}/complete", UriKind.Relative), null)).EnsureSuccessStatusCode();

        var published = factory.Published.OfType<Contracts.TaskCompleted>().Where(m => m.HouseholdId == h.Id).ToList();
        var completed = Assert.Single(published);
        Assert.Equal((home.Id, "Wynieść śmieci", h.Other), (completed.ListId, completed.Title, completed.CompletedBy));

        var done = (await h.TasksAsync(h.Member, "?status=Done")).Select(t => t.Id).ToHashSet();
        Assert.Equal([shared, secret], done);
        (await h.As(h.Member).PostAsync(new Uri($"{h.Api}/tasks/{shared}/reopen", UriKind.Relative), null)).EnsureSuccessStatusCode();
        Assert.Equal(TodoStatus.Open, (await h.TasksAsync(h.Member, $"?list={home.Id}")).Single().Status);
    }

    [Fact]
    public async Task Filters_make_the_mine_and_today_views()
    {
        var h = await AHousehold.SwitchedOnAsync(factory);
        var home = await h.HomeListAsync();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        await h.AddTaskAsync(h.Admin, home.Id, "Moje na dziś", new { assigneeId = h.Member, dueDate = today, dueTime = "18:00:00" });
        await h.AddTaskAsync(h.Admin, home.Id, "Zaległe", new { dueDate = today.AddDays(-3) });
        await h.AddTaskAsync(h.Admin, home.Id, "Jutro", new { assigneeId = h.Member, dueDate = today.AddDays(1) });
        await h.AddTaskAsync(h.Admin, home.Id, "Kiedyś");

        var mine = await h.TasksAsync(h.Member, "?assignee=me");
        var dueToday = await h.TasksAsync(h.Member, $"?dueBy={today:yyyy-MM-dd}");

        Assert.Equal(["Moje na dziś", "Jutro"], mine.Select(t => t.Title));
        Assert.Equal(["Moje na dziś", "Zaległe"], dueToday.Select(t => t.Title));
        Assert.Equal(new TimeOnly(18, 0), dueToday[0].DueTime);
    }

    [Fact]
    public async Task A_due_time_needs_a_due_date()
    {
        var h = await AHousehold.SwitchedOnAsync(factory);
        var home = await h.HomeListAsync();

        var response = await h.PostTaskAsync(h.Member, home.Id, "X", new { dueTime = "10:00:00" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Moving_puts_a_task_before_another_or_at_the_end_and_onto_another_list()
    {
        var h = await AHousehold.SwitchedOnAsync(factory);
        var home = await h.HomeListAsync();
        var a = await h.AddTaskAsync(h.Member, home.Id, "A");
        var b = await h.AddTaskAsync(h.Member, home.Id, "B");
        var c = await h.AddTaskAsync(h.Member, home.Id, "C");

        await Move(c, home.Id, b);
        Assert.Equal(["A", "C", "B"], (await h.TasksAsync(h.Member, $"?list={home.Id}")).Select(t => t.Title));
        await Move(a, home.Id, null);
        Assert.Equal(["C", "B", "A"], (await h.TasksAsync(h.Member, $"?list={home.Id}")).Select(t => t.Title));
        await Move(b, home.Id, c);
        Assert.Equal(["B", "C", "A"], (await h.TasksAsync(h.Member, $"?list={home.Id}")).Select(t => t.Title));

        var mine = await h.CreateListAsync(h.Member, "Moje", "Private");
        await Move(a, mine, null);
        Assert.Equal(["A"], (await h.TasksAsync(h.Member, $"?list={mine}")).Select(t => t.Title));
        Assert.DoesNotContain(await h.TasksAsync(h.Owner), t => t.Id == a);

        async Task Move(Guid task, Guid list, Guid? before) =>
            (await h.As(h.Member).PostAsJsonAsync($"{h.Api}/tasks/{task}/move", new { listId = list, beforeTaskId = before })).EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Editing_changes_the_whole_form()
    {
        var h = await AHousehold.SwitchedOnAsync(factory);
        var home = await h.HomeListAsync();
        var task = await h.AddTaskAsync(h.Member, home.Id, "Kupić mleko");

        var response = await h.As(h.Other).PutAsJsonAsync($"{h.Api}/tasks/{task}",
            new { title = "Kupić chleb", notes = "razowy", dueDate = "2026-12-24", priority = "High", assigneeId = h.Other });

        response.EnsureSuccessStatusCode();
        var view = (await h.TasksAsync(h.Member)).Single();
        Assert.Equal(("Kupić chleb", "razowy", new DateOnly(2026, 12, 24), TaskPriority.High, (Guid?)h.Other),
            (view.Title, view.Notes, view.DueDate, view.Priority, view.AssigneeId));
    }

    [Fact]
    public async Task A_shared_task_is_deleted_by_its_author_or_an_admin()
    {
        var h = await AHousehold.SwitchedOnAsync(factory);
        var home = await h.HomeListAsync();
        var first = await h.AddTaskAsync(h.Member, home.Id, "Pierwsze");
        var second = await h.AddTaskAsync(h.Member, home.Id, "Drugie");

        var byOther = await h.As(h.Other).DeleteAsync(new Uri($"{h.Api}/tasks/{first}", UriKind.Relative));
        var byAuthor = await h.As(h.Member).DeleteAsync(new Uri($"{h.Api}/tasks/{first}", UriKind.Relative));
        var byAdmin = await h.As(h.Admin).DeleteAsync(new Uri($"{h.Api}/tasks/{second}", UriKind.Relative));

        Assert.Equal(HttpStatusCode.Forbidden, byOther.StatusCode);
        Assert.Equal("task.forbidden", await AHousehold.CodeOf(byOther));
        Assert.Equal(HttpStatusCode.NoContent, byAuthor.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, byAdmin.StatusCode);
        Assert.Empty(await h.TasksAsync(h.Member));
    }

    [Fact]
    public async Task Deleting_a_list_deletes_its_tasks()
    {
        var h = await AHousehold.SwitchedOnAsync(factory);
        var mine = await h.CreateListAsync(h.Member, "Moje", "Private");
        await h.AddTaskAsync(h.Member, mine, "X");

        (await h.As(h.Member).DeleteAsync(new Uri($"{h.Api}/lists/{mine}", UriKind.Relative))).EnsureSuccessStatusCode();

        Assert.DoesNotContain(await h.ListsAsync(h.Member), l => l.Id == mine);
        Assert.Empty(await h.TasksAsync(h.Member));
    }

    [Fact]
    public async Task T5_someone_who_leaves_loses_assignments_and_private_lists_which_come_back_or_go_after_30_days()
    {
        var h = await AHousehold.SwitchedOnAsync(factory);
        var home = await h.HomeListAsync();
        var assigned = await h.AddTaskAsync(h.Admin, home.Id, "Odkurzyć", new { assigneeId = h.Member });
        var mine = await h.CreateListAsync(h.Member, "Moje", "Private");

        await Leave(h.Member);
        await Eventually(async () => (await h.TasksAsync(h.Admin)).Single(t => t.Id == assigned).AssigneeId is null);

        await Rejoin(h.Member);
        await Eventually(async () => (await h.ListsAsync(h.Member)).Any(l => l.Id == mine));

        await Leave(h.Member);
        await Eventually(async () => await PrivateListHidden());
        factory.Time.Advance(TimeSpan.FromDays(31));
        try
        {
            await using var scope = factory.Services.CreateAsyncScope();
            var purged = await scope.ServiceProvider.GetRequiredService<ISender>().Send(new PurgeLeftListsCommand());
            Assert.True(purged.Value >= 1);
        }
        finally
        {
            factory.Time.Reset();
        }

        await Rejoin(h.Member);
        Assert.DoesNotContain(await h.ListsAsync(h.Member), l => l.Id == mine);

        async Task Leave(Guid userId)
        {
            factory.Households.SetRole(h.Id, userId, null);
            await factory.Harness.Bus.Publish(new MemberLeft(h.Id, userId, MemberLeftReason.Left));
        }

        async Task Rejoin(Guid userId)
        {
            factory.Households.SetRole(h.Id, userId, HouseholdRole.Member);
            await factory.Harness.Bus.Publish(new MemberJoined(h.Id, userId, HouseholdRole.Member));
        }

        async Task<bool> PrivateListHidden()
        {
            await using var scope = factory.Services.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<Todo.Infrastructure.Persistence.TodoDbContext>();
            return (await db.Lists.FindAsync(mine))?.OwnerLeftAt is not null;
        }
    }

    /// <summary>
    /// Waits up to 10 s for something the bus does asynchronously. Not the harness's own <c>Any</c>: once the harness
    /// has seen the bus go quiet it answers at once, so later tests in the class would not wait at all.
    /// </summary>
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
