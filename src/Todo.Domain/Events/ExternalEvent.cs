namespace Todo.Domain.Events;

/// <summary>
/// One VEVENT of an imported calendar. Times are instants; for a recurring event <see cref="Rrule"/> is expanded in the
/// event's own zone so the local time stays put across daylight saving changes.
/// </summary>
public sealed record ExternalEvent(
    string Uid,
    string Title,
    string? Description,
    string? Location,
    string TimeZoneId,
    DateTimeOffset StartUtc,
    DateTimeOffset EndUtc,
    bool IsAllDay,
    string? Rrule,
    IReadOnlyList<DateOnly> ExcludedDates)
{
    /// <summary>Start and end of every occurrence overlapping [fromUtc, toUtc); at most <see cref="RecurrenceEvaluator.MaxOccurrences"/>.</summary>
    public IReadOnlyList<(DateTimeOffset StartUtc, DateTimeOffset EndUtc)> OccurrencesBetween(DateTimeOffset fromUtc, DateTimeOffset toUtc)
    {
        if (Rrule is null || RecurrenceRule.Create(Rrule) is not { IsSuccess: true } rule)
        {
            return EndUtc >= fromUtc && StartUtc < toUtc ? [(StartUtc, EndUtc)] : [];
        }

        var startDate = EventTime.LocalDate(StartUtc, TimeZoneId);
        var startTime = EventTime.LocalTime(StartUtc, TimeZoneId);
        var length = EndUtc - StartUtc;
        var dates = RecurrenceEvaluator.Dates(
            rule.Value.Value, startDate, null, DateOnly.FromDateTime(fromUtc.UtcDateTime).AddDays(-2), DateOnly.FromDateTime(toUtc.UtcDateTime).AddDays(1));

        return dates
            .Where(d => !ExcludedDates.Contains(d))
            .Select(d => { var s = EventTime.ToUtc(d, startTime, TimeZoneId); return (StartUtc: s, EndUtc: s + length); })
            .Where(o => o.EndUtc >= fromUtc && o.StartUtc < toUtc)
            .Take(RecurrenceEvaluator.MaxOccurrences)
            .ToList();
    }
}
