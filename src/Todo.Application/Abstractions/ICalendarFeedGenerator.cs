using Todo.Application.Common;
using Todo.Domain.Events;

namespace Todo.Application.Abstractions;

/// <summary>Writes an RFC 5545 calendar.</summary>
public interface ICalendarFeedGenerator
{
    string Generate(
        string name, IReadOnlyList<CalendarEvent> events, IReadOnlyList<(EventSeries Series, EventOccurrence Occurrence)> occurrences,
        IReadOnlyList<TaskView> tasks, DateTimeOffset now);
}
