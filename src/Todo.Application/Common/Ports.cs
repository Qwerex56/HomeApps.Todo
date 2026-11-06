using Todo.Domain.Lists;
using Todo.Domain.Tasks;

namespace Todo.Application.Common;

/// <summary>Loads and stores <see cref="TodoList"/>. Saving is the unit of work's job; deleting a list deletes its tasks.</summary>
public interface ITodoListRepository
{
    Task<TodoList?> GetAsync(Guid householdId, Guid listId, CancellationToken cancellationToken);

    /// <summary>T5: every private list of one person in one household, hidden ones included.</summary>
    Task<IReadOnlyList<TodoList>> ListPrivateOfAsync(Guid householdId, Guid ownerId, CancellationToken cancellationToken);

    /// <summary>T5: private lists whose owner left at least <see cref="TodoList.KeptAfterOwnerLeft"/> before <paramref name="now"/>.</summary>
    Task<IReadOnlyList<TodoList>> ListDueForPurgeAsync(DateTimeOffset now, CancellationToken cancellationToken);

    void Add(TodoList list);

    void Remove(TodoList list);
}

/// <summary>Loads and stores <see cref="TodoTask"/>.</summary>
public interface ITodoTaskRepository
{
    Task<TodoTask?> GetAsync(Guid householdId, Guid taskId, CancellationToken cancellationToken);

    /// <summary>Tasks of the household assigned to <paramref name="userId"/>.</summary>
    Task<IReadOnlyList<TodoTask>> ListAssignedToAsync(Guid householdId, Guid userId, CancellationToken cancellationToken);

    /// <summary>The highest position on the list, leaving out <paramref name="exceptTaskId"/>; null on an empty list.</summary>
    Task<decimal?> LastPositionAsync(Guid listId, Guid? exceptTaskId, CancellationToken cancellationToken);

    /// <summary>The highest position below <paramref name="position"/> on the list, leaving out <paramref name="exceptTaskId"/>.</summary>
    Task<decimal?> PositionBeforeAsync(Guid listId, decimal position, Guid exceptTaskId, CancellationToken cancellationToken);

    void Add(TodoTask task);

    void Remove(TodoTask task);
}

/// <summary>Read-only views projected straight from the database, already limited to what the user may see.</summary>
public interface ITodoReader
{
    /// <summary>Shared lists and the user's own visible private lists, archived ones included, by name.</summary>
    Task<IReadOnlyList<ListView>> ListsAsync(Guid householdId, Guid userId, CancellationToken cancellationToken);

    /// <summary>Tasks on lists the user may see, matching <paramref name="filter"/>: open first, then by list and position.</summary>
    Task<IReadOnlyList<TaskView>> TasksAsync(Guid householdId, Guid userId, TaskFilter filter, CancellationToken cancellationToken);
}

/// <param name="ListId">Only this list.</param>
/// <param name="AssigneeId">Only tasks assigned to this person.</param>
/// <param name="DueBy">Only open tasks due on this day or before (the household's "today" comes from the client).</param>
/// <param name="Status">Only open or only done tasks.</param>
public sealed record TaskFilter(Guid? ListId = null, Guid? AssigneeId = null, DateOnly? DueBy = null, TodoStatus? Status = null);
