using FluentResults;
using Todo.Domain.Common;

namespace Todo.Domain.Events;

/// <summary>An external ICS calendar a user imported by URL. The SSRF check is the Application layer's job (P4).</summary>
public sealed class ExternalCalendarImport
{
    public const int MaxUrlLength = 2048;
    public const int MaxNameLength = 100;

    private ExternalCalendarImport() { }

    public Guid Id { get; private set; }

    public Guid HouseholdId { get; private set; }

    public Guid UserId { get; private set; }

    public string Url { get; private set; } = null!;

    public string Name { get; private set; } = null!;

    public string Color { get; private set; } = null!;

    public ExternalCalendarVisibility Visibility { get; private set; }

    public string? CachedIcsData { get; private set; }

    public DateTimeOffset? LastSyncedAt { get; private set; }

    public DateTimeOffset? LastSyncFailedAt { get; private set; }

    public string? LastErrorMessage { get; private set; }

    public int SyncFailureCount { get; private set; }

    public DateTimeOffset? OwnerLeftAt { get; private set; }

    public bool IsPaused => OwnerLeftAt is not null;

    public static Result<ExternalCalendarImport> Create(
        Guid householdId, Guid userId, string? url, string? name, string? color, ExternalCalendarVisibility visibility, TimeProvider time)
    {
        ArgumentNullException.ThrowIfNull(time);
        var trimmed = url?.Trim() ?? string.Empty;
        var label = name?.Trim() ?? string.Empty;
        if (trimmed.Length is 0 or > MaxUrlLength || !Uri.TryCreate(trimmed, UriKind.Absolute, out _)
            || label.Length is 0 or > MaxNameLength || Text.HasControlCharacters(label))
        {
            return Result.Fail<ExternalCalendarImport>(PlannerErrors.InvalidImport);
        }

        return new ExternalCalendarImport
        {
            Id = Guid.CreateVersion7(time.GetUtcNow()),
            HouseholdId = householdId,
            UserId = userId,
            Url = trimmed,
            Name = label,
            Color = Text.OrNull(color) ?? "#3b82f6",
            Visibility = visibility,
        };
    }

    public void Synced(string icsData, DateTimeOffset now)
    {
        CachedIcsData = icsData;
        LastSyncedAt = now;
        LastSyncFailedAt = null;
        LastErrorMessage = null;
        SyncFailureCount = 0;
    }

    public void SyncFailed(string message, DateTimeOffset now)
    {
        LastSyncFailedAt = now;
        LastErrorMessage = message.Length > 500 ? message[..500] : message;
        SyncFailureCount++;
    }

    public void OwnerLeft(DateTimeOffset now) => OwnerLeftAt ??= now;

    public void OwnerReturned() => OwnerLeftAt = null;
}
