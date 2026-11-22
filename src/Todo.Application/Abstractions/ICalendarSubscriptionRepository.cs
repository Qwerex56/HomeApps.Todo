using Todo.Domain.Events;

namespace Todo.Application.Abstractions;

/// <summary>Loads and stores <see cref="CalendarSubscription"/>. Saving is the unit of work's job.</summary>
public interface ICalendarSubscriptionRepository
{
    Task<CalendarSubscription?> GetAsync(Guid householdId, Guid userId, Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<CalendarSubscription>> ListAsync(Guid householdId, Guid userId, CancellationToken cancellationToken);

    Task<IReadOnlyList<CalendarSubscription>> ListOfUserAsync(Guid userId, CancellationToken cancellationToken);

    Task<CalendarSubscription?> FindByTokenHashAsync(string tokenHash, CancellationToken cancellationToken);

    void Add(CalendarSubscription subscription);

    void Remove(CalendarSubscription subscription);
}
