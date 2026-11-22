using Microsoft.EntityFrameworkCore;
using Todo.Application.Abstractions;
using Todo.Domain.Events;

namespace Todo.Infrastructure.Persistence;

internal sealed class CalendarSubscriptionRepository(TodoDbContext db) : ICalendarSubscriptionRepository
{
    public Task<CalendarSubscription?> GetAsync(Guid householdId, Guid userId, Guid id, CancellationToken cancellationToken) =>
        db.CalendarSubscriptions.FirstOrDefaultAsync(s => s.Id == id && s.HouseholdId == householdId && s.UserId == userId, cancellationToken);

    public async Task<IReadOnlyList<CalendarSubscription>> ListAsync(Guid householdId, Guid userId, CancellationToken cancellationToken) =>
        await db.CalendarSubscriptions.Where(s => s.HouseholdId == householdId && s.UserId == userId && s.IsActive)
            .OrderBy(s => s.CreatedAt).AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<CalendarSubscription>> ListOfUserAsync(Guid userId, CancellationToken cancellationToken) =>
        await db.CalendarSubscriptions.Where(s => s.UserId == userId).ToListAsync(cancellationToken);

    public Task<CalendarSubscription?> FindByTokenHashAsync(string tokenHash, CancellationToken cancellationToken) =>
        db.CalendarSubscriptions.AsNoTracking().FirstOrDefaultAsync(s => s.TokenHash == tokenHash, cancellationToken);

    public void Add(CalendarSubscription subscription) => db.CalendarSubscriptions.Add(subscription);

    public void Remove(CalendarSubscription subscription) => db.CalendarSubscriptions.Remove(subscription);
}
