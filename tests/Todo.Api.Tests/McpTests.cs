using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using HomeApps.BuildingBlocks.Testing;
using HomeApps.Contracts.Households;
using ModelContextProtocol.Client;
using Todo.Application;
using Todo.Domain.Tasks;

namespace Todo.Api.Tests;

/// <summary>The MCP tools as JARVIS calls them: with the person's delegated token, households and people by name.</summary>
public sealed class McpTests(TodoFactory factory) : IClassFixture<TodoFactory>
{
    private const string Endpoint = "plugins/todo/mcp";

    [Fact]
    public async Task The_manifest_and_the_server_name_the_same_tools()
    {
        var json = await factory.CreateClient().GetFromJsonAsync<JsonNode>("/plugins/todo/manifest");
        var h = await AHousehold.SwitchedOnAsync(factory);
        await using var mcp = await Connect(h.Member);

        var tools = await mcp.ListToolsAsync();

        Assert.Equal(["add_task", "my_tasks", "complete_task"], json!["mcp"]!["tools"]!.AsArray().Select(t => t!.GetValue<string>()));
        Assert.Equal(TodoPlugin.Manifest.Mcp!.Tools.Order(StringComparer.Ordinal), tools.Select(t => t.Name).Order(StringComparer.Ordinal));
        Assert.True(tools.Single(t => t.Name == "my_tasks").ProtocolTool.Annotations!.ReadOnlyHint);
        Assert.False(tools.Single(t => t.Name == "complete_task").ProtocolTool.Annotations!.DestructiveHint);
    }

    [Fact]
    public async Task Add_task_with_names_only_lands_on_the_one_list_with_due_date_and_assignee()
    {
        var h = await AHousehold.SwitchedOnAsync(factory);
        await using var mcp = await Connect(h.Member);
        var tomorrow = Today().AddDays(1);

        var added = await mcp.CallTextAsync("add_task", new Dictionary<string, object?>
        {
            ["title"] = "Kupić mleko", ["due"] = $"{tomorrow:yyyy-MM-dd} 18:00", ["assignee"] = "olek",
        });

        Assert.Equal(($"Added \"Kupić mleko\" to the list Dom in Mieszkanie, due {tomorrow:yyyy-MM-dd} 18:00, for Olek.", false), added);
        var task = Assert.Single(await h.TasksAsync(h.Member));
        Assert.Equal(("Kupić mleko", tomorrow, new TimeOnly(18, 0), h.Other, TaskPriority.Normal), (task.Title, task.DueDate!.Value, task.DueTime!.Value, task.AssigneeId, task.Priority));
        Assert.Equal(h.Member, task.CreatedBy);
    }

    [Fact]
    public async Task With_two_lists_add_task_asks_which_and_takes_a_name()
    {
        var h = await AHousehold.SwitchedOnAsync(factory);
        await h.CreateListAsync(h.Admin, "Zakupy", "Shared");
        await using var mcp = await Connect(h.Member);

        var which = await mcp.CallTextAsync("add_task", new Dictionary<string, object?> { ["title"] = "Chleb" });
        var added = await mcp.CallTextAsync("add_task", new Dictionary<string, object?> { ["title"] = "Chleb", ["list"] = "zakupy", ["assignee"] = "me", ["household"] = "Mieszkanie" });
        var unknown = await mcp.CallTextAsync("add_task", new Dictionary<string, object?> { ["title"] = "Chleb", ["list"] = "Zakupy", ["assignee"] = "Zenek" });

        Assert.True(which.IsError);
        Assert.Contains("list.ambiguous", which.Text, StringComparison.Ordinal);
        Assert.Contains("Dom, Zakupy", which.Text, StringComparison.Ordinal);
        Assert.Equal(("Added \"Chleb\" to the list Zakupy in Mieszkanie, for Ania.", false), added);
        Assert.True(unknown.IsError);
        Assert.Contains("person.not_found", unknown.Text, StringComparison.Ordinal);
    }

    [Fact]
    public async Task A_guest_may_not_add_and_a_bad_date_is_explained()
    {
        var h = await AHousehold.SwitchedOnAsync(factory);
        await using var guest = await Connect(h.Guest);
        await using var member = await Connect(h.Member);

        var refused = await guest.CallTextAsync("add_task", new Dictionary<string, object?> { ["title"] = "Też coś" });
        var badDate = await member.CallTextAsync("add_task", new Dictionary<string, object?> { ["title"] = "Coś", ["due"] = "jutro" });

        Assert.True(refused.IsError);
        Assert.Contains("plugin.forbidden", refused.Text, StringComparison.Ordinal);
        Assert.True(badDate.IsError);
        Assert.StartsWith("Invalid due:", badDate.Text, StringComparison.Ordinal);
        Assert.Empty(await h.TasksAsync(h.Member));
    }

    [Fact]
    public async Task My_tasks_lists_open_tasks_assigned_to_me_across_households_and_filters_by_day()
    {
        var h = await AHousehold.SwitchedOnAsync(factory);
        var second = await AHousehold.SwitchedOnAsync(factory);
        factory.Households.HouseholdNames[second.Id] = "Działka";
        factory.Households.SetRole(second.Id, h.Member, HouseholdRole.Member);
        var home = await h.HomeListAsync();
        var cottage = await second.HomeListAsync();
        var today = Today();
        await h.AddTaskAsync(h.Member, home.Id, "Wynieść śmieci", new { assigneeId = h.Member, dueDate = today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) });
        await h.AddTaskAsync(h.Member, home.Id, "Umyć okna", new { assigneeId = h.Member, dueDate = today.AddDays(10).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) });
        await h.AddTaskAsync(h.Member, home.Id, "Nie moje", new { assigneeId = h.Other });
        await second.AddTaskAsync(h.Member, cottage.Id, "Skosić trawę", new { assigneeId = h.Member });
        await using var mcp = await Connect(h.Member);

        var all = await mcp.CallTextAsync("my_tasks", new Dictionary<string, object?>());
        var forToday = await mcp.CallTextAsync("my_tasks", new Dictionary<string, object?> { ["when"] = "today", ["household"] = "mieszkanie" });

        Assert.False(all.IsError);
        Assert.Contains("Mieszkanie:", all.Text, StringComparison.Ordinal);
        Assert.Contains("Działka:", all.Text, StringComparison.Ordinal);
        Assert.Contains("- Skosić trawę (list Dom) id ", all.Text, StringComparison.Ordinal);
        Assert.Contains("Umyć okna", all.Text, StringComparison.Ordinal);
        Assert.DoesNotContain("Nie moje", all.Text, StringComparison.Ordinal);
        Assert.Contains($"- Wynieść śmieci (list Dom, due {today:yyyy-MM-dd}) id ", forToday.Text, StringComparison.Ordinal);
        Assert.DoesNotContain("Umyć okna", forToday.Text, StringComparison.Ordinal);
        Assert.DoesNotContain("Skosić", forToday.Text, StringComparison.Ordinal);
    }

    [Fact]
    public async Task My_tasks_skips_households_where_todo_is_off()
    {
        var h = await AHousehold.SwitchedOnAsync(factory);
        var off = Guid.NewGuid();
        factory.Households.HouseholdNames[off] = "Bez zadań";
        factory.Households.SetRole(off, h.Member, HouseholdRole.Owner);
        await h.AddTaskAsync(h.Member, (await h.HomeListAsync()).Id, "Podlać kwiaty", new { assigneeId = h.Member });
        await using var mcp = await Connect(h.Member);

        var all = await mcp.CallTextAsync("my_tasks", new Dictionary<string, object?>());
        var named = await mcp.CallTextAsync("my_tasks", new Dictionary<string, object?> { ["household"] = "Bez zadań" });

        Assert.False(all.IsError);
        Assert.Contains("Podlać kwiaty", all.Text, StringComparison.Ordinal);
        Assert.True(named.IsError);
        Assert.Contains("plugin.disabled", named.Text, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Complete_task_by_title_or_id_and_never_guesses_between_two()
    {
        var h = await AHousehold.SwitchedOnAsync(factory);
        var home = await h.HomeListAsync();
        var milk = await h.AddTaskAsync(h.Member, home.Id, "Kupić mleko");
        await h.AddTaskAsync(h.Member, home.Id, "Odkurzyć");
        var twin = await h.AddTaskAsync(h.Other, home.Id, "Odkurzyć");
        await using var mcp = await Connect(h.Member);

        var byTitle = await mcp.CallTextAsync("complete_task", new Dictionary<string, object?> { ["task"] = "kupić MLEKO" });
        var twice = await mcp.CallTextAsync("complete_task", new Dictionary<string, object?> { ["task"] = "Odkurzyć" });
        var byId = await mcp.CallTextAsync("complete_task", new Dictionary<string, object?> { ["task"] = twin.ToString() });

        Assert.Equal(("Done: \"Kupić mleko\" in Mieszkanie.", false), byTitle);
        Assert.True(twice.IsError);
        Assert.Contains("task.ambiguous", twice.Text, StringComparison.Ordinal);
        Assert.False(byId.IsError);
        var tasks = await h.TasksAsync(h.Member);
        Assert.Equal(TodoStatus.Done, tasks.Single(t => t.Id == milk).Status);
        Assert.Equal(TodoStatus.Done, tasks.Single(t => t.Id == twin).Status);
        Assert.Equal(TodoStatus.Open, tasks.Single(t => t.Title == "Odkurzyć" && t.Id != twin).Status);
    }

    [Fact]
    public async Task An_assistant_may_change_tasks_but_never_switch_todo()
    {
        var h = await AHousehold.SwitchedOnAsync(factory);

        var response = await factory.CreateAssistantClient(h.Owner).PutAsJsonAsync($"{h.Api}/enablement", new { enabled = false });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("assistant.not_allowed", await AHousehold.CodeOf(response));
    }

    private Task<McpClient> Connect(Guid userId) => McpTestClient.ConnectAsync(factory.CreateAssistantClient(userId), Endpoint);

    private DateOnly Today() => DateOnly.FromDateTime(factory.Time.GetLocalNow().DateTime);
}
