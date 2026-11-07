using Microsoft.EntityFrameworkCore;
using Todo.Application.Abstractions;
using Todo.Domain.Lists;

namespace Todo.Infrastructure.Persistence;

internal sealed class TodoListRepository(TodoDbContext db) : ITodoListRepository
{
    public Task<TodoList?> GetAsync(Guid householdId, Guid listId, CancellationToken cancellationToken) =>
        db.Lists.FirstOrDefaultAsync(l => l.Id == listId && l.HouseholdId == householdId, cancellationToken);

    public async Task<IReadOnlyList<TodoList>> ListPrivateOfAsync(Guid householdId, Guid ownerId, CancellationToken cancellationToken) =>
        await db.Lists
            .Where(l => l.HouseholdId == householdId && l.OwnerId == ownerId && l.Visibility == ListVisibility.Private)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<TodoList>> ListDueForPurgeAsync(DateTimeOffset now, CancellationToken cancellationToken)
    {
        var leftBefore = now - TodoList.KeptAfterOwnerLeft;
        return await db.Lists.Where(l => l.OwnerLeftAt != null && l.OwnerLeftAt <= leftBefore).ToListAsync(cancellationToken);
    }

    public void Add(TodoList list) => db.Lists.Add(list);

    public void Remove(TodoList list) => db.Lists.Remove(list);
}
