using Todo.Domain.Events;

namespace Todo.Application.Abstractions;

/// <summary>Loads and stores <see cref="EventSeries"/> and its <see cref="EventOverride"/> records. Removing a series removes its overrides.</summary>
public interface IEventSeriesRepository
{
    Task<EventSeries?> GetAsync(Guid householdId, Guid seriesId, CancellationToken cancellationToken);

    Task<IReadOnlyList<EventOverride>> OverridesAsync(Guid seriesId, CancellationToken cancellationToken);

    void Add(EventSeries series);

    void Remove(EventSeries series);

    void AddOverride(EventOverride change);

    void RemoveOverrides(IEnumerable<EventOverride> changes);
}
