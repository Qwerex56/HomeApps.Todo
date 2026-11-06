using Todo.Application.Common;

namespace Todo.Application.Abstractions;

/// <summary>Read-only views projected straight from the database, already limited to what the user may see.</summary>
public interface ITodoReader
{
    /// <summary>Shared lists and the user's own visible private lists, archived ones included, by name.</summary>
    Task<IReadOnlyList<ListView>> ListsAsync(Guid householdId, Guid userId, CancellationToken cancellationToken);

    /// <summary>Tasks on lists the user may see, matching <paramref name="filter"/>: open first, then by list and position.</summary>
    Task<IReadOnlyList<TaskView>> TasksAsync(Guid householdId, Guid userId, TaskFilter filter, CancellationToken cancellationToken);
}
