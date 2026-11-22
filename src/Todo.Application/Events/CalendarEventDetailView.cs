using Todo.Domain.Events;

namespace Todo.Application.Events;

/// <summary>One event, or one occurrence of a series (then <c>Id</c> is the series id and <c>OccurrenceDate</c> is set). <c>Rrule</c> is set for series.</summary>
public sealed record CalendarEventDetailView(
    Guid Id,
    Guid? SeriesId,
    DateOnly? OccurrenceDate,
    string Title,
    string? Description,
    string? Location,
    string TimeZoneId,
    DateTimeOffset StartUtc,
    DateTimeOffset EndUtc,
    bool IsAllDay,
    bool IsPrivate,
    IReadOnlyList<Guid> Attendees,
    IReadOnlyList<int> ReminderMinutes,
    string? Rrule,
    Guid CreatedBy)
{
    public static CalendarEventDetailView Of(CalendarEvent e) => new(
        e.Id, e.SeriesId, null, e.Title.Value, e.Description, e.Location, e.TimeZoneId, e.StartUtc, e.EndUtc, e.IsAllDay, e.IsPrivate,
        e.Attendees, e.Reminders.Select(r => r.MinutesBefore).ToList(), null, e.CreatedBy);

    public static CalendarEventDetailView Of(EventSeries s, EventOccurrence o) => new(
        s.Id, s.Id, o.OriginalDate, o.Title, o.Description, o.Location, o.TimeZoneId, o.StartUtc, o.EndUtc, o.IsAllDay, s.IsPrivate,
        [], o.Reminders, s.Rule.Value, s.CreatedBy);
}
