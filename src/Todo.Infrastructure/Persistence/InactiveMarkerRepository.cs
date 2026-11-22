using Microsoft.EntityFrameworkCore;
using Todo.Application.Abstractions;
using Todo.Domain.Households;

namespace Todo.Infrastructure.Persistence;

internal sealed class InactiveMarkerRepository(TodoDbContext db, TimeProvider time) : IInactiveMarkerRepository
{
    public async Task MarkAsync(Guid? householdId, Guid? userId, CancellationToken cancellationToken)
    {
        if (!await db.InactiveMarkers.AnyAsync(m => m.HouseholdId == householdId && m.UserId == userId, cancellationToken))
        {
            db.InactiveMarkers.Add(new InactiveMarker(householdId, userId, time.GetUtcNow()));
        }
    }

    public async Task ClearAsync(Guid? householdId, Guid? userId, CancellationToken cancellationToken) =>
        db.InactiveMarkers.RemoveRange(await db.InactiveMarkers.Where(m => m.HouseholdId == householdId && m.UserId == userId).ToListAsync(cancellationToken));

    public Task<bool> IsInactiveAsync(Guid householdId, Guid userId, CancellationToken cancellationToken) =>
        db.InactiveMarkers.AnyAsync(
            m => (m.HouseholdId == householdId && m.UserId == null) || (m.HouseholdId == null && m.UserId == userId) || (m.HouseholdId == householdId && m.UserId == userId),
            cancellationToken);
}
