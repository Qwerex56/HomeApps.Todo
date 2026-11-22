using Microsoft.EntityFrameworkCore;
using Todo.Application.Abstractions;
using Todo.Domain.Events;

namespace Todo.Infrastructure.Persistence;

internal sealed class CalendarEventRepository(TodoDbContext db) : ICalendarEventRepository
{
    public Task<CalendarEvent?> GetAsync(Guid householdId, Guid eventId, CancellationToken cancellationToken) =>
        db.CalendarEvents.FirstOrDefaultAsync(e => e.Id == eventId && e.HouseholdId == householdId, cancellationToken);

    public void Add(CalendarEvent calendarEvent) => db.CalendarEvents.Add(calendarEvent);

    public void Remove(CalendarEvent calendarEvent) => db.CalendarEvents.Remove(calendarEvent);
}
