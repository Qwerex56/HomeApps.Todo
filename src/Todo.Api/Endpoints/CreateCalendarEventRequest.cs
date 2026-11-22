using Todo.Application.Events;
using Todo.Application.Events.CreateCalendarEvent;

namespace Todo.Api.Endpoints;

/// <summary>Body of <c>POST /calendar/events</c>; <c>Rrule</c> makes the event a recurring series.</summary>
internal sealed record CreateCalendarEventRequest(
    string? Title,
    string? Description,
    string? Location,
    string? TimeZoneId,
    DateTimeOffset StartUtc,
    DateTimeOffset EndUtc,
    bool IsAllDay,
    bool IsPrivate,
    IReadOnlyList<Guid>? Attendees,
    IReadOnlyList<int>? ReminderMinutes,
    string? Rrule)
{
    public CreateCalendarEventCommand ToCommand(Guid householdId) => new(householdId,
        new CalendarEventInput(Title, Description, Location, TimeZoneId, StartUtc, EndUtc, IsAllDay, IsPrivate, Attendees, ReminderMinutes, Rrule));
}
