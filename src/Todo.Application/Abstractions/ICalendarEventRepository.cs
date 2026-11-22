using Todo.Domain.Events;

namespace Todo.Application.Abstractions;

/// <summary>Loads and stores <see cref="CalendarEvent"/>. Saving is the unit of work's job.</summary>
public interface ICalendarEventRepository
{
    Task<CalendarEvent?> GetAsync(Guid householdId, Guid eventId, CancellationToken cancellationToken);

    void Add(CalendarEvent calendarEvent);

    void Remove(CalendarEvent calendarEvent);
}
