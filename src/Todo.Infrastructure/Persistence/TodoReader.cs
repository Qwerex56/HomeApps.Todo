using Microsoft.EntityFrameworkCore;
using Todo.Application.Abstractions;
using Todo.Application.Common;
using Todo.Domain.Lists;
using Todo.Domain.Tasks;

namespace Todo.Infrastructure.Persistence;

internal sealed class TodoReader(TodoDbContext db) : ITodoReader
{
    public async Task<IReadOnlyList<ListView>> ListsAsync(Guid householdId, Guid userId, CancellationToken cancellationToken)
    {
        var rows = await Visible(householdId, userId)
            .Select(l => new { List = l, Open = db.Tasks.Count(t => t.ListId == l.Id && t.Status == TodoStatus.Open) })
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return rows
            .OrderBy(r => r.List.Name.Value, StringComparer.CurrentCultureIgnoreCase)
            .Select(r => new ListView(r.List.Id, r.List.Name.Value, r.List.Visibility, r.List.OwnerId, r.List.IsArchived, r.Open, r.List.CreatedAt))
            .ToList();
    }

    public async Task<IReadOnlyList<TaskView>> TasksAsync(Guid householdId, Guid userId, TaskFilter filter, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(filter);
        var lists = Visible(householdId, userId);
        var tasks = db.Tasks.Where(t => t.HouseholdId == householdId && lists.Any(l => l.Id == t.ListId));

        if (filter.ListId is { } listId)
        {
            tasks = tasks.Where(t => t.ListId == listId);
        }

        if (filter.AssigneeId is { } assigneeId)
        {
            tasks = tasks.Where(t => t.AssigneeId == assigneeId);
        }

        if (filter.DueBy is { } dueBy)
        {
            tasks = tasks.Where(t => t.Status == TodoStatus.Open && t.Due != null && t.Due.Date <= dueBy);
        }

        if (filter.Status is { } status)
        {
            tasks = tasks.Where(t => t.Status == status);
        }

        var found = await tasks
            .OrderBy(t => t.Status == TodoStatus.Done)
            .ThenBy(t => t.ListId)
            .ThenBy(t => t.Position)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return found
            .Select(t => new TaskView(
                t.Id, t.ListId, t.Title.Value, t.Notes?.Value, t.Due?.Date, t.Due?.Time, t.AssigneeId, t.Priority, t.Status,
                t.Position, t.CreatedBy, t.CreatedAt, t.CompletedBy, t.CompletedAt))
            .ToList();
    }

    /// <summary>Shared lists and the user's own private lists while they are in the household (T5).</summary>
    private IQueryable<TodoList> Visible(Guid householdId, Guid userId) =>
        db.Lists.Where(l => l.HouseholdId == householdId
            && (l.Visibility == ListVisibility.Shared || (l.OwnerId == userId && l.OwnerLeftAt == null)));
}
