using Todo.Application.Common;
using Todo.Domain.Events;

namespace Todo.Application.Events;

/// <summary>
/// One entry of a calendar window. An event or an occurrence has <c>StartUtc</c>/<c>EndUtc</c>; a task has its due
/// <c>Date</c> (and <c>Time</c> when set), which are the household's local day, not UTC. An occurrence has the series
/// id and its original local date; <c>Id</c> of an occurrence is the series id.
/// </summary>
public sealed record CalendarItemView(
    CalendarItemType ItemType,
    Guid Id,
    Guid? SeriesId,
    bool IsOccurrence,
    DateOnly? OccurrenceDate,
    string Title,
    string? Description,
    string? Location,
    string? TimeZoneId,
    DateTimeOffset? StartUtc,
    DateTimeOffset? EndUtc,
    DateOnly? Date,
    TimeOnly? Time,
    bool IsAllDay,
    bool IsPrivate,
    IReadOnlyList<Guid> Attendees,
    Guid CreatedBy,
    bool? IsDone,
    string? Color = null)
{
    public static CalendarItemView Of(CalendarEvent e) => new(
        CalendarItemType.Event, e.Id, e.SeriesId, false, null, e.Title.Value, e.Description, e.Location, e.TimeZoneId, e.StartUtc, e.EndUtc,
        null, null, e.IsAllDay, e.IsPrivate, e.Attendees, e.CreatedBy, null);

    public static CalendarItemView Of(EventSeries s, EventOccurrence o) => new(
        CalendarItemType.Event, s.Id, s.Id, true, o.OriginalDate, o.Title, o.Description, o.Location, o.TimeZoneId, o.StartUtc, o.EndUtc,
        null, null, o.IsAllDay, s.IsPrivate, [], s.CreatedBy, null);

    /// <summary>One occurrence of an imported event, read-only; <c>Id</c> is the import. A "busy" view carries no details.</summary>
    public static CalendarItemView Of(ExternalCalendarImport calendar, ExternalEvent e, DateTimeOffset startUtc, DateTimeOffset endUtc, bool showDetails) => new(
        CalendarItemType.External, calendar.Id, null, false, null, showDetails ? e.Title : "Zajęty", showDetails ? e.Description : null,
        showDetails ? e.Location : null, e.TimeZoneId, startUtc, endUtc, null, null, e.IsAllDay, false, [calendar.UserId], calendar.UserId, null, calendar.Color);

    public static CalendarItemView Of(TaskView t) => new(
        CalendarItemType.Task, t.Id, null, false, null, t.Title, t.Notes, null, null, null, null, t.DueDate, t.DueTime, false, false,
        t.AssigneeId is { } a ? [a] : [], t.CreatedBy, t.Status == Todo.Domain.Tasks.TodoStatus.Done);
}
