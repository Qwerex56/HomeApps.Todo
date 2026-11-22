using Microsoft.EntityFrameworkCore;
using Todo.Application.Abstractions;
using Todo.Domain.Events;

namespace Todo.Infrastructure.Persistence;

internal sealed class EventSeriesRepository(TodoDbContext db) : IEventSeriesRepository
{
    public Task<EventSeries?> GetAsync(Guid householdId, Guid seriesId, CancellationToken cancellationToken) =>
        db.EventSeries.FirstOrDefaultAsync(s => s.Id == seriesId && s.HouseholdId == householdId, cancellationToken);

    public async Task<IReadOnlyList<EventOverride>> OverridesAsync(Guid seriesId, CancellationToken cancellationToken) =>
        await db.EventOverrides.Where(o => o.SeriesId == seriesId).ToListAsync(cancellationToken);

    public void Add(EventSeries series) => db.EventSeries.Add(series);

    public void Remove(EventSeries series) => db.EventSeries.Remove(series);

    public void AddOverride(EventOverride change) => db.EventOverrides.Add(change);

    public void RemoveOverrides(IEnumerable<EventOverride> changes) => db.EventOverrides.RemoveRange(changes);
}
