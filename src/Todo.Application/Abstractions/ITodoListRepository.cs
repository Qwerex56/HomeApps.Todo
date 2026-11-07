using Todo.Domain.Lists;

namespace Todo.Application.Abstractions;

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
