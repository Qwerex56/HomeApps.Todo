namespace Todo.Domain.Events;

/// <summary>One expanded occurrence of a series, with its override (if any) already applied.</summary>
public sealed record EventOccurrence(
    Guid SeriesId,
    DateOnly OriginalDate,
    string Title,
    string? Description,
    string? Location,
    string TimeZoneId,
    DateTimeOffset StartUtc,
    DateTimeOffset EndUtc,
    bool IsAllDay,
    IReadOnlyList<int> Reminders);
