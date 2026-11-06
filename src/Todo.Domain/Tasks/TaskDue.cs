namespace Todo.Domain.Tasks;

/// <summary>
/// When a task is due: a day in the household's own calendar, optionally with a time. No time zone on purpose; "today"
/// is the household's today, which the caller passes in.
/// </summary>
public sealed record TaskDue(DateOnly Date, TimeOnly? Time = null)
{
    /// <summary>Due on <paramref name="today"/> or already overdue.</summary>
    public bool IsDueBy(DateOnly today) => Date <= today;
}
