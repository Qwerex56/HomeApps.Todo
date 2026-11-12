using System.ComponentModel;
using System.Globalization;
using System.Text;
using FluentResults;
using HomeApps.BuildingBlocks.Application;
using HomeApps.BuildingBlocks.Application.Abstractions;
using HomeApps.BuildingBlocks.Mcp;
using HomeApps.Plugins;
using HomeApps.Plugins.Hosting;
using MediatR;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using Todo.Application.Common;
using Todo.Application.Lists.GetLists;
using Todo.Application.Tasks;
using Todo.Application.Tasks.CompleteTask;
using Todo.Application.Tasks.CreateTask;
using Todo.Application.Tasks.GetTasks;
using Todo.Domain.Tasks;
using Result = FluentResults.Result;

namespace Todo.Api.Mcp;

/// <summary>
/// Todo's MCP tools for JARVIS (jarvis-v1.md, section 4): the same requests as the HTTP API, so the same permission
/// checks. Households, lists and people are named the way a person names them, or by id; a name that fits several
/// is never guessed. "Today" is the server's local date: the plugin knows no household time zone yet.
/// </summary>
[McpServerToolType]
internal sealed class TaskTools(ISender sender, IHouseholdDirectory directory, ICurrentUser user, TimeProvider time)
{
    [McpServerTool(Name = "add_task", Title = "Add a task", Destructive = false, OpenWorld = false)]
    [Description("Adds a task to a to-do list of a household. Lists are shared with the household or private to the person.")]
    public async Task<CallToolResult> AddTask(
        [Description("What to do, up to 200 characters.")] string title,
        [Description("The list's name or id. Leave out when the household has one list; otherwise the error names them.")] string? list = null,
        [Description("The household's name or id. Leave out when the person is in one household.")] string? household = null,
        [Description("Due date, e.g. 2026-11-13, optionally with a time: 2026-11-13 18:00. Resolve words like \"tomorrow\" first.")] string? due = null,
        [Description("Who should do it: a member's name or id, or \"me\". Leave out for nobody in particular.")] string? assignee = null,
        CancellationToken cancellationToken = default)
    {
        var home = await directory.FindHouseholdAsync(household, cancellationToken);
        if (home.IsFailed)
        {
            return ToolResult.Error(home.Errors);
        }

        var lists = await sender.Send(new GetListsQuery(home.Value.Id), cancellationToken);
        if (lists.IsFailed)
        {
            return ToolResult.Error(lists.Errors);
        }

        var target = NameLookup.Find(lists.Value.Where(l => !l.Archived).ToList(), list, l => l.Id, l => l.Name, "list");
        var when = Due.Parse(due);
        var who = await AssigneeAsync(home.Value.Id, assignee, cancellationToken);
        var input = Result.Merge(target.ToResult(), when.ToResult(), who.ToResult());
        if (input.IsFailed)
        {
            return ToolResult.Error(input.Errors);
        }

        var task = new TaskInput(title, null, when.Value.Date, when.Value.Time, TaskPriority.Normal, who.Value?.UserId);
        var created = await sender.Send(new CreateTaskCommand(home.Value.Id, target.Value.Id, task), cancellationToken);
        return ToolResult.From(created, _ =>
        {
            var text = new StringBuilder(string.Create(CultureInfo.InvariantCulture, $"Added \"{title.Trim()}\" to the list {target.Value.Name} in {home.Value.Name}"));
            if (when.Value.Date is not null)
            {
                text.Append(CultureInfo.InvariantCulture, $", due {Due.Describe(when.Value.Date, when.Value.Time)}");
            }

            if (who.Value is { } person)
            {
                text.Append(CultureInfo.InvariantCulture, $", for {person.Name}");
            }

            return text.Append('.').ToString();
        });
    }

    [McpServerTool(Name = "my_tasks", Title = "My tasks", ReadOnly = true, Idempotent = true, OpenWorld = false)]
    [Description("Lists the open tasks assigned to the person, with their ids, from one household or from all of them.")]
    public async Task<CallToolResult> MyTasks(
        [Description("\"today\" (due today or overdue), \"week\" (due within 7 days) or \"all\" (default), or a date like 2026-11-13 for due by then.")] string? when = null,
        [Description("The household's name or id. Leave out for all of the person's households.")] string? household = null,
        CancellationToken cancellationToken = default)
    {
        var dueBy = DueBy(when);
        if (dueBy.IsFailed)
        {
            return ToolResult.Error(dueBy.Errors);
        }

        var homes = await directory.HouseholdsToSearchAsync(household, cancellationToken);
        if (homes.IsFailed)
        {
            return ToolResult.Error(homes.Errors);
        }

        var named = !string.IsNullOrWhiteSpace(household);
        var text = new StringBuilder();
        foreach (var home in homes.Value)
        {
            var tasks = await sender.Send(new GetTasksQuery(home.Id, null, true, dueBy.Value, TodoStatus.Open), cancellationToken);
            var lists = tasks.IsSuccess ? await sender.Send(new GetListsQuery(home.Id), cancellationToken) : tasks.ToResult<IReadOnlyList<ListView>>();
            if (lists.IsFailed)
            {
                if (named || !PluginErrors.IsOffOrForbidden(lists.Errors))
                {
                    return ToolResult.Error(lists.Errors);
                }

                continue;
            }

            if (tasks.Value.Count == 0)
            {
                continue;
            }

            var listNames = lists.Value.ToDictionary(l => l.Id, l => l.Name);
            if (homes.Value.Count > 1)
            {
                text.AppendLine(CultureInfo.InvariantCulture, $"{home.Name}:");
            }

            foreach (var task in tasks.Value)
            {
                text.AppendLine(Line(task, listNames.GetValueOrDefault(task.ListId, "?")));
            }
        }

        return ToolResult.Text(text.Length == 0 ? "No open tasks assigned to the person." : text.ToString().TrimEnd());
    }

    [McpServerTool(Name = "complete_task", Title = "Complete a task", Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Ticks off an open task, found by its title or by the id my_tasks shows.")]
    public async Task<CallToolResult> CompleteTask(
        [Description("The task's title or id.")] string task,
        [Description("The household's name or id. Leave out to look in all of the person's households.")] string? household = null,
        CancellationToken cancellationToken = default)
    {
        var homes = await directory.HouseholdsToSearchAsync(household, cancellationToken);
        if (homes.IsFailed)
        {
            return ToolResult.Error(homes.Errors);
        }

        var open = new List<(HouseholdName Home, TaskView Task)>();
        foreach (var home in homes.Value)
        {
            var tasks = await sender.Send(new GetTasksQuery(home.Id, null, false, null, TodoStatus.Open), cancellationToken);
            if (tasks.IsFailed)
            {
                if (!string.IsNullOrWhiteSpace(household) || !PluginErrors.IsOffOrForbidden(tasks.Errors))
                {
                    return ToolResult.Error(tasks.Errors);
                }

                continue;
            }

            open.AddRange(tasks.Value.Select(t => (home, t)));
        }

        var found = NameLookup.Find(open, string.IsNullOrWhiteSpace(task) ? "?" : task, t => t.Task.Id, t => t.Task.Title, "task", "my_tasks");
        if (found.IsFailed)
        {
            return ToolResult.Error(found.Errors);
        }

        var (where, done) = found.Value;
        var completed = await sender.Send(new CompleteTaskCommand(where.Id, done.Id), cancellationToken);
        return ToolResult.From(completed, $"Done: \"{done.Title}\" in {where.Name}.");
    }

    private async Task<Result<MemberName?>> AssigneeAsync(Guid householdId, string? assignee, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(assignee))
        {
            return Result.Ok<MemberName?>(null);
        }

        var found = assignee.Trim().Equals("me", StringComparison.OrdinalIgnoreCase)
            ? await directory.FindPersonAsync(householdId, user.RequiredUserId.ToString(), cancellationToken)
            : await directory.FindPersonAsync(householdId, assignee, cancellationToken);
        return found.Map<MemberName?>(m => m);
    }

    private Result<DateOnly?> DueBy(string? when)
    {
        var today = DateOnly.FromDateTime(time.GetLocalNow().DateTime);
        return when?.Trim().ToLowerInvariant() switch
        {
            null or "" or "all" => Result.Ok<DateOnly?>(null),
            "today" => Result.Ok<DateOnly?>(today),
            "week" => Result.Ok<DateOnly?>(today.AddDays(6)),
            var other when DateOnly.TryParseExact(other, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var day) => Result.Ok<DateOnly?>(day),
            _ => Result.Fail(ValidationErrors.ForField("when", "Use today, week, all or a date like 2026-11-13.")),
        };
    }

    private static string Line(TaskView task, string list)
    {
        var due = Due.Describe(task.DueDate, task.DueTime);
        return string.Create(CultureInfo.InvariantCulture,
            $"- {task.Title} (list {list}{(due.Length == 0 ? string.Empty : $", due {due}")}{(task.Priority == TaskPriority.High ? ", high priority" : string.Empty)}) id {task.Id}");
    }
}
