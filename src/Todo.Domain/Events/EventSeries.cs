using FluentResults;
using Todo.Domain.Common;

namespace Todo.Domain.Events;

/// <summary>
/// A recurring event stored as a rule, not as rows (ADR decision 3). Occurrences are expanded in the requested window;
/// changes to single occurrences are <see cref="EventOverride"/> records. The local time and zone keep "16:00" at 16:00
/// across daylight saving changes.
/// </summary>
public sealed class EventSeries
{
    private EventSeries() { }

    public Guid Id { get; private set; }

    public Guid HouseholdId { get; private set; }

    public Guid CreatedBy { get; private set; }

    public EventTitle Title { get; private set; } = null!;

    public string? Description { get; private set; }

    public string? Location { get; private set; }

    public string TimeZoneId { get; private set; } = null!;

    public TimeOnly LocalStartTime { get; private set; }

    public TimeOnly LocalEndTime { get; private set; }

    public bool IsAllDay { get; private set; }

    public bool IsPrivate { get; private set; }

    public RecurrenceRule Rule { get; private set; } = null!;

    public DateTimeOffset SeriesStartUtc { get; private set; }

    /// <summary>Where the series ends (the UNTIL of a split series); null while it runs on.</summary>
    public DateTimeOffset? SeriesEndUtc { get; private set; }

    public DateTimeOffset? OwnerLeftAt { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public static Result<EventSeries> Create(
        Guid householdId,
        Actor actor,
        string? title,
        string? description,
        string? location,
        string? timeZoneId,
        TimeOnly localStartTime,
        TimeOnly localEndTime,
        bool isAllDay,
        bool isPrivate,
        string? rrule,
        DateTimeOffset seriesStartUtc,
        DateTimeOffset? seriesEndUtc,
        TimeProvider time)
    {
        ArgumentNullException.ThrowIfNull(actor);
        ArgumentNullException.ThrowIfNull(time);
        if (householdId == Guid.Empty)
        {
            throw new ArgumentException("The household id cannot be empty.", nameof(householdId));
        }

        var validTitle = EventTitle.Create(title);
        var zone = EventTime.CheckZone(timeZoneId);
        var rule = RecurrenceRule.Create(rrule);
        var range = seriesEndUtc < seriesStartUtc ? Result.Fail(PlannerErrors.InvalidDateRange) : Result.Ok();
        var check = Result.Merge(validTitle, zone, rule, range);
        if (check.IsFailed)
        {
            return check.ToResult<EventSeries>();
        }

        var now = time.GetUtcNow();
        return new EventSeries
        {
            Id = Guid.CreateVersion7(now),
            HouseholdId = householdId,
            CreatedBy = actor.UserId,
            Title = validTitle.Value,
            Description = Text.OrNull(description),
            Location = Text.OrNull(location),
            TimeZoneId = timeZoneId!,
            LocalStartTime = localStartTime,
            LocalEndTime = localEndTime,
            IsAllDay = isAllDay,
            IsPrivate = isPrivate,
            Rule = rule.Value,
            SeriesStartUtc = seriesStartUtc.ToUniversalTime(),
            SeriesEndUtc = seriesEndUtc?.ToUniversalTime(),
            CreatedAt = now,
            UpdatedAt = now,
        };
    }

    /// <summary>Ends the series at <paramref name="endUtc"/> (ThisAndFuture splits the series there).</summary>
    public Result EndAt(DateTimeOffset endUtc, TimeProvider time)
    {
        ArgumentNullException.ThrowIfNull(time);
        if (endUtc < SeriesStartUtc)
        {
            return Result.Fail(PlannerErrors.InvalidDateRange);
        }

        SeriesEndUtc = endUtc.ToUniversalTime();
        UpdatedAt = time.GetUtcNow();
        return Result.Ok();
    }

    public void OwnerLeft(Guid userId, TimeProvider time)
    {
        ArgumentNullException.ThrowIfNull(time);
        if (IsPrivate && CreatedBy == userId && OwnerLeftAt is null)
        {
            OwnerLeftAt = UpdatedAt = time.GetUtcNow();
        }
    }

    public void OwnerReturned(TimeProvider time)
    {
        ArgumentNullException.ThrowIfNull(time);
        if (OwnerLeftAt is not null)
        {
            OwnerLeftAt = null;
            UpdatedAt = time.GetUtcNow();
        }
    }
}
