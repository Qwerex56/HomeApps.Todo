using Microsoft.EntityFrameworkCore;
using Todo.Application.Abstractions;
using Todo.Domain.Tasks;

namespace Todo.Infrastructure.Persistence;

internal sealed class TodoTaskRepository(TodoDbContext db) : ITodoTaskRepository
{
    public Task<TodoTask?> GetAsync(Guid householdId, Guid taskId, CancellationToken cancellationToken) =>
        db.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && t.HouseholdId == householdId, cancellationToken);

    public async Task<IReadOnlyList<TodoTask>> ListAssignedToAsync(Guid householdId, Guid userId, CancellationToken cancellationToken) =>
        await db.Tasks.Where(t => t.HouseholdId == householdId && t.AssigneeId == userId).ToListAsync(cancellationToken);

    public Task<decimal?> LastPositionAsync(Guid listId, Guid? exceptTaskId, CancellationToken cancellationToken) =>
        db.Tasks.Where(t => t.ListId == listId && t.Id != exceptTaskId).MaxAsync(t => (decimal?)t.Position, cancellationToken);

    public Task<decimal?> PositionBeforeAsync(Guid listId, decimal position, Guid exceptTaskId, CancellationToken cancellationToken) =>
        db.Tasks.Where(t => t.ListId == listId && t.Position < position && t.Id != exceptTaskId)
            .MaxAsync(t => (decimal?)t.Position, cancellationToken);

    public void Add(TodoTask task) => db.Tasks.Add(task);

    public void Remove(TodoTask task) => db.Tasks.Remove(task);
}
