using Todo.Application.Common;
using Todo.Domain.Events;

namespace Todo.Application.Abstractions;

/// <summary>
/// Read-only calendar data for a window, already limited to what the user may see: private events of others and of
/// users who left are left out; for a guest only events that attend them or are for the whole household.
/// </summary>
public interface ICalendarReader
{
    /// <summary>Single events overlapping [<paramref name="fromUtc"/>, <paramref name="toUtc"/>).</summary>
    Task<IReadOnlyList<CalendarEvent>> EventsAsync(
        Guid householdId, Guid userId, bool isGuest, DateTimeOffset fromUtc, DateTimeOffset toUtc, CancellationToken cancellationToken);

    /// <summary>Series that may have occurrences in the window, with their overrides.</summary>
    Task<IReadOnlyList<(EventSeries Series, IReadOnlyList<EventOverride> Overrides)>> SeriesAsync(
        Guid householdId, Guid userId, bool isGuest, DateTimeOffset fromUtc, DateTimeOffset toUtc, CancellationToken cancellationToken);

    /// <summary>Tasks with a due day in [firstDay, lastDay] on lists the user may see.</summary>
    Task<IReadOnlyList<TaskView>> TasksDueAsync(
        Guid householdId, Guid userId, DateOnly firstDay, DateOnly lastDay, CancellationToken cancellationToken);
}
