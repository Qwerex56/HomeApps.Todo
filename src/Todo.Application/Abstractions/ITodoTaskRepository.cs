using Todo.Domain.Tasks;

namespace Todo.Application.Abstractions;

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
