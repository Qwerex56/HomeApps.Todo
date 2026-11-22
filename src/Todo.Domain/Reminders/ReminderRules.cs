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
}
