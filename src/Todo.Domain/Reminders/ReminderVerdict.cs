namespace Todo.Domain.Reminders;

/// <summary>What the scheduler does with one reminder moment (ADR 3.8).</summary>
public enum ReminderVerdict
{
    /// <summary>Not in this scan: still in the future, or already handled by an earlier scan.</summary>
    Skip,

    /// <summary>Fires now.</summary>
    Send,

    /// <summary>More than <see cref="ReminderRules.StaleAfter"/> late: dropped, never sent.</summary>
    Drop,
}
