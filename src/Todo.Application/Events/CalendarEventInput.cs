namespace Todo.Application.Events;

/// <summary>
/// What the caller sends for an event. With <c>Rrule</c> the event is a series for the whole household (or private):
/// a series keeps no attendees or reminders of its own, so they must be empty.
/// </summary>
public sealed record CalendarEventInput(
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
    string? Rrule);
