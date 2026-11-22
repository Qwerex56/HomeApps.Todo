using Todo.Application.Events;
using Todo.Application.Events.UpdateCalendarEvent;

namespace Todo.Api.Endpoints;

/// <summary>Body of <c>PUT /calendar/events/{id}</c>: the whole single event.</summary>
internal sealed record UpdateCalendarEventRequest(
    string? Title,
    string? Description,
    string? Location,
    string? TimeZoneId,
    DateTimeOffset StartUtc,
    DateTimeOffset EndUtc,
    bool IsAllDay,
    bool IsPrivate,
    IReadOnlyList<Guid>? Attendees,
    IReadOnlyList<int>? ReminderMinutes)
{
    public UpdateCalendarEventCommand ToCommand(Guid householdId, Guid eventId) => new(householdId, eventId,
        new CalendarEventInput(Title, Description, Location, TimeZoneId, StartUtc, EndUtc, IsAllDay, IsPrivate, Attendees, ReminderMinutes, null));
}
