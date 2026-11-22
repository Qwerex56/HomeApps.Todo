namespace Todo.Domain.Reminders;

/// <summary>Pure rules of the reminder scheduler (ADR 3.8): which moments fire and which minutes apply to whom.</summary>
public static class ReminderRules
{
    public static readonly TimeSpan StaleAfter = TimeSpan.FromMinutes(60);

    /// <summary>A moment fires when it falls in (<paramref name="from"/>, <paramref name="now"/>]; one more than an hour late is dropped.</summary>
    public static ReminderVerdict Judge(DateTimeOffset at, DateTimeOffset from, DateTimeOffset now)
    {
        if (at <= from || at > now)
        {
            return ReminderVerdict.Skip;
        }

        return now - at > StaleAfter ? ReminderVerdict.Drop : ReminderVerdict.Send;
    }

    /// <summary>
    /// Minutes before the start a user is reminded: their own set for the event replaces everything (even when empty),
    /// else the reminders on the event, else their defaults.
    /// </summary>
    public static IReadOnlyList<int> MinutesFor(IReadOnlyList<int> onEvent, IReadOnlyList<int>? userOverride, IReadOnlyList<int> userDefaults) =>
        userOverride ?? (onEvent.Count > 0 ? onEvent : userDefaults);

    /// <summary>Local time of the morning digest.</summary>
    public static readonly TimeOnly DigestTime = new(7, 30);

    /// <summary>
    /// The instant a local date and time falls on in <paramref name="zone"/>. A time skipped by a spring-forward jump moves
    /// one hour on; a time that happens twice in autumn means the first one.
    /// </summary>
    public static DateTimeOffset LocalToUtc(DateOnly date, TimeOnly time, TimeZoneInfo zone)
    {
        ArgumentNullException.ThrowIfNull(zone);
        var local = date.ToDateTime(time, DateTimeKind.Unspecified);
        if (zone.IsInvalidTime(local))
        {
            local = local.AddHours(1);
        }

        var offset = zone.IsAmbiguousTime(local) ? zone.GetAmbiguousTimeOffsets(local).Max() : zone.GetUtcOffset(local);
        return new DateTimeOffset(local, offset).ToUniversalTime();
    }

    /// <summary>The zone for an IANA id, or the fallback when the id is empty or unknown.</summary>
    public static TimeZoneInfo ResolveZone(string? id, string fallbackId)
    {
        foreach (var candidate in new[] { id, fallbackId })
        {
            if (!string.IsNullOrWhiteSpace(candidate) && TimeZoneInfo.TryFindSystemTimeZoneById(candidate, out var zone))
            {
                return zone;
            }
        }

        return TimeZoneInfo.Utc;
    }
}
