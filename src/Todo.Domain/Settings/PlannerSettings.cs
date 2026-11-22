using FluentResults;
using Todo.Domain.Events;

namespace Todo.Domain.Settings;

/// <summary>One user's planner preferences in one household: default reminders and the first day of the week.</summary>
public sealed class PlannerSettings
{
    private PlannerSettings() { }

    public PlannerSettings(Guid householdId, Guid userId, DateTimeOffset now)
    {
        Id = Guid.CreateVersion7(now);
        HouseholdId = householdId;
        UserId = userId;
        FirstDayOfWeek = DayOfWeek.Monday;
        UpdatedAt = now;
    }

    public Guid Id { get; private set; }

    public Guid HouseholdId { get; private set; }

    public Guid UserId { get; private set; }

    public List<EventReminder> DefaultEventReminders { get; private set; } = [];

    public List<EventReminder> DefaultTaskReminders { get; private set; } = [];

    public DayOfWeek FirstDayOfWeek { get; private set; }

    /// <summary>IANA time zone for task and digest reminders; <c>null</c> means the configured default.</summary>
    public string? TimeZoneId { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public Result Update(IEnumerable<int>? eventReminders, IEnumerable<int>? taskReminders, DayOfWeek firstDay, TimeProvider time, string? timeZoneId = null)
    {
        ArgumentNullException.ThrowIfNull(time);
        var events = EventReminder.CreateMany(eventReminders);
        var tasks = EventReminder.CreateMany(taskReminders);
        if (!string.IsNullOrWhiteSpace(timeZoneId) && !TimeZoneInfo.TryFindSystemTimeZoneById(timeZoneId, out _))
        {
            return Result.Fail($"Unknown time zone '{timeZoneId}'.");
        }

        var check = Result.Merge((ResultBase)events, tasks);
        if (check.IsFailed)
        {
            return check;
        }

        DefaultEventReminders = events.Value;
        DefaultTaskReminders = tasks.Value;
        FirstDayOfWeek = firstDay;
        TimeZoneId = string.IsNullOrWhiteSpace(timeZoneId) ? TimeZoneId : timeZoneId;
        UpdatedAt = time.GetUtcNow();
        return Result.Ok();
    }
}
