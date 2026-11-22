using Microsoft.EntityFrameworkCore;
using Todo.Application.Abstractions;
using Todo.Domain.Events;

namespace Todo.Infrastructure.Persistence;

internal sealed class ExternalCalendarRepository(TodoDbContext db) : IExternalCalendarRepository
{
    public Task<ExternalCalendarImport?> GetAsync(Guid householdId, Guid id, CancellationToken cancellationToken) =>
        db.ExternalCalendarImports.FirstOrDefaultAsync(c => c.Id == id && c.HouseholdId == householdId, cancellationToken);

    public async Task<IReadOnlyList<ExternalCalendarImport>> ListOfUserAsync(Guid householdId, Guid userId, CancellationToken cancellationToken) =>
        await db.ExternalCalendarImports.Where(c => c.HouseholdId == householdId && c.UserId == userId && c.OwnerLeftAt == null)
            .OrderBy(c => c.Name).AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<ExternalCalendarImport>> ListAllOfUserAsync(Guid? householdId, Guid userId, CancellationToken cancellationToken) =>
        await db.ExternalCalendarImports.Where(c => c.UserId == userId && (householdId == null || c.HouseholdId == householdId)).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<ExternalCalendarImport>> ListVisibleAsync(Guid householdId, Guid viewerId, CancellationToken cancellationToken) =>
        await db.ExternalCalendarImports
            .Where(c => c.HouseholdId == householdId && c.OwnerLeftAt == null && (c.UserId == viewerId || c.Visibility != ExternalCalendarVisibility.OnlyMe))
            .AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<ExternalCalendarImport>> ListDueForSyncAsync(DateTimeOffset now, CancellationToken cancellationToken)
    {
        var before = now - ExternalCalendarImport.SyncInterval;
        var candidates = await db.ExternalCalendarImports
            .Where(c => c.OwnerLeftAt == null
                && (c.LastSyncedAt == null || c.LastSyncedAt <= before) && (c.LastSyncFailedAt == null || c.LastSyncFailedAt <= before))
            .ToListAsync(cancellationToken);
        return candidates.Where(c => c.IsDueForSync(now)).ToList();
    }

    public async Task<IReadOnlyList<ExternalCalendarImport>> ListDueForPurgeAsync(DateTimeOffset now, CancellationToken cancellationToken)
    {
        var leftBefore = now - ExternalCalendarImport.KeptAfterOwnerLeft;
        return await db.ExternalCalendarImports.Where(c => c.OwnerLeftAt != null && c.OwnerLeftAt <= leftBefore).ToListAsync(cancellationToken);
    }

    public void Add(ExternalCalendarImport import) => db.ExternalCalendarImports.Add(import);

    public void Remove(ExternalCalendarImport import) => db.ExternalCalendarImports.Remove(import);
}
