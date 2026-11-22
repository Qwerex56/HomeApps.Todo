using FluentResults;

namespace Todo.Domain.Events;

/// <summary>One user's own reminders on an event, replacing the defaults for them only.</summary>
public sealed class EventReminderOverride
{
    private EventReminderOverride() { }

    public EventReminderOverride(Guid eventId, Guid userId, DateTimeOffset now)
    {
        Id = Guid.CreateVersion7(now);
        EventId = eventId;
        UserId = userId;
        UpdatedAt = now;
    }

    public Guid Id { get; private set; }

    public Guid EventId { get; private set; }

    public Guid UserId { get; private set; }

    public List<EventReminder> Reminders { get; private set; } = [];

    public DateTimeOffset UpdatedAt { get; private set; }

    public Result Set(IEnumerable<int>? minutes, TimeProvider time)
    {
        ArgumentNullException.ThrowIfNull(time);
        var rems = EventReminder.CreateMany(minutes);
        if (rems.IsFailed)
        {
            return rems.ToResult();
        }

        Reminders = rems.Value;
        UpdatedAt = time.GetUtcNow();
        return Result.Ok();
    }
}
