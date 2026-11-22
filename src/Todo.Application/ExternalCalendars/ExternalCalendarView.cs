using Todo.Domain.Events;

namespace Todo.Application.ExternalCalendars;

/// <summary>An import as its owner sees it. The URL is secret and never leaves the server.</summary>
public sealed record ExternalCalendarView(
    Guid Id, string Name, string Color, ExternalCalendarVisibility Visibility,
    DateTimeOffset? LastSyncedAt, DateTimeOffset? LastSyncFailedAt, string? LastErrorMessage, bool IsStale)
{
    public static ExternalCalendarView Of(ExternalCalendarImport c) =>
        new(c.Id, c.Name, c.Color, c.Visibility, c.LastSyncedAt, c.LastSyncFailedAt, c.LastErrorMessage, c.IsStale);
}
