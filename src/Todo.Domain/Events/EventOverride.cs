using FluentResults;
using Todo.Domain.Common;

namespace Todo.Domain.Events;

/// <summary>A change to one occurrence of a series (RFC 5545 RECURRENCE-ID): cancelled, or with its own title, time or reminders.</summary>
public sealed class EventOverride
{
    private EventOverride() { }

    public Guid Id { get; private set; }

    public Guid SeriesId { get; private set; }

    public DateOnly OriginalInstanceDate { get; private set; }

    public bool IsCancelled { get; private set; }

    public EventTitle? OverrideTitle { get; private set; }

    public DateTimeOffset? OverrideStartUtc { get; private set; }

    public DateTimeOffset? OverrideEndUtc { get; private set; }

    public List<EventReminder>? OverrideReminders { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public static EventOverride Cancel(Guid seriesId, DateOnly instanceDate, TimeProvider time)
    {
        ArgumentNullException.ThrowIfNull(time);
        var now = time.GetUtcNow();
        return new EventOverride
        {
            Id = Guid.CreateVersion7(now),
            SeriesId = seriesId,
            OriginalInstanceDate = instanceDate,
            IsCancelled = true,
            UpdatedAt = now,
        };
    }

    public static Result<EventOverride> Change(
        Guid seriesId,
        DateOnly instanceDate,
        string? title,
        DateTimeOffset? startUtc,
        DateTimeOffset? endUtc,
        IEnumerable<int>? reminders,
        TimeProvider time)
    {
        ArgumentNullException.ThrowIfNull(time);
        var created = new EventOverride { SeriesId = seriesId, OriginalInstanceDate = instanceDate };
        var result = created.Replace(title, startUtc, endUtc, reminders, time);
        if (result.IsFailed)
        {
            return result.ToResult<EventOverride>();
        }

        created.Id = Guid.CreateVersion7(time.GetUtcNow());
        return created;
    }

    /// <summary>Replaces what this override changes and un-cancels the occurrence. Null fields keep the series' value.</summary>
    public Result Replace(string? title, DateTimeOffset? startUtc, DateTimeOffset? endUtc, IEnumerable<int>? reminders, TimeProvider time)
    {
        ArgumentNullException.ThrowIfNull(time);
        Result<EventTitle>? validTitle = title is null ? null : EventTitle.Create(title);
        var rems = reminders is null ? null : EventReminder.CreateMany(reminders);
        var range = startUtc is { } s && endUtc is { } e && e < s ? Result.Fail(PlannerErrors.InvalidDateRange) : Result.Ok();
        var check = Result.Merge(range, validTitle ?? Result.Ok(), rems ?? Result.Ok());
        if (check.IsFailed)
        {
            return check;
        }

        IsCancelled = false;
        OverrideTitle = validTitle?.Value;
        OverrideStartUtc = startUtc?.ToUniversalTime();
        OverrideEndUtc = endUtc?.ToUniversalTime();
        OverrideReminders = rems?.Value;
        UpdatedAt = time.GetUtcNow();
        return Result.Ok();
    }

    public void CancelOccurrence(TimeProvider time)
    {
        ArgumentNullException.ThrowIfNull(time);
        IsCancelled = true;
        OverrideTitle = null;
        OverrideStartUtc = OverrideEndUtc = null;
        OverrideReminders = null;
        UpdatedAt = time.GetUtcNow();
    }
}
