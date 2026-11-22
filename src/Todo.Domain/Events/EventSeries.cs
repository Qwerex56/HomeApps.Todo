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

    /// <summary>Creates a series from the first occurrence's instants; its local time and day are read in the zone.</summary>
    public static Result<EventSeries> Create(
        Guid householdId,
        Actor actor,
        string? title,
        string? description,
        string? location,
        string? timeZoneId,
        DateTimeOffset startUtc,
        DateTimeOffset endUtc,
        bool isAllDay,
        bool isPrivate,
        string? rrule,
        DateTimeOffset? seriesEndUtc,
        TimeProvider time)
    {
        var zone = EventTime.CheckZone(timeZoneId);
        if (zone.IsFailed)
        {
            return zone.ToResult<EventSeries>();
        }

        if (endUtc < startUtc || endUtc - startUtc > TimeSpan.FromDays(1))
        {
            return Result.Fail<EventSeries>(PlannerErrors.InvalidDateRange);
        }

        return Create(householdId, actor, title, description, location, timeZoneId, EventTime.LocalTime(startUtc, timeZoneId!),
            EventTime.LocalTime(endUtc, timeZoneId!), isAllDay, isPrivate, rrule, startUtc, seriesEndUtc, time);
    }

    public bool CanSee(Actor actor)
    {
        ArgumentNullException.ThrowIfNull(actor);
        return !IsPrivate || (actor.UserId == CreatedBy && OwnerLeftAt is null);
    }

    /// <summary>A series is for the whole household unless private, so any member edits it; a private one only its author.</summary>
    public Result CanEdit(Actor actor) =>
        !CanSee(actor) ? Result.Fail(PlannerErrors.EventNotFound) : Result.Ok();

    /// <summary>The author or a household admin; a private series only its author.</summary>
    public Result CanDelete(Actor actor)
    {
        if (!CanSee(actor))
        {
            return Result.Fail(PlannerErrors.EventNotFound);
        }

        return actor.UserId == CreatedBy || (!IsPrivate && actor.Manages) ? Result.Ok() : Result.Fail(PlannerErrors.CannotDeleteEvent);
    }

    /// <summary>The first local day of the series.</summary>
    public DateOnly StartDate => EventTime.LocalDate(SeriesStartUtc, TimeZoneId);

    /// <summary>"All occurrences": new texts, local time of day and rule. The first day stays.</summary>
    public Result Edit(
        Actor actor, string? title, string? description, string? location, TimeOnly localStartTime, TimeOnly localEndTime,
        bool isAllDay, string? rrule, TimeProvider time)
    {
        ArgumentNullException.ThrowIfNull(time);
        var allowed = CanEdit(actor);
        if (allowed.IsFailed)
        {
            return allowed;
        }

        var validTitle = EventTitle.Create(title);
        var rule = RecurrenceRule.Create(rrule);
        var check = Result.Merge(validTitle, rule);
        if (check.IsFailed)
        {
            return check;
        }

        var first = StartDate;
        Title = validTitle.Value;
        Description = Text.OrNull(description);
        Location = Text.OrNull(location);
        LocalStartTime = localStartTime;
        LocalEndTime = localEndTime;
        IsAllDay = isAllDay;
        Rule = rule.Value;
        SeriesStartUtc = EventTime.ToUtc(first, localStartTime, TimeZoneId);
        UpdatedAt = time.GetUtcNow();
        return Result.Ok();
    }

    /// <summary>The occurrence on <paramref name="date"/> (local day) with its override applied; null when there is none or it is cancelled.</summary>
    public EventOccurrence? OccurrenceOn(DateOnly date, EventOverride? change)
    {
        var onRule = RecurrenceEvaluator.Dates(Rule.Value, StartDate, SeriesEndUtc is { } e ? EventTime.LocalDate(e, TimeZoneId) : null, date, date).Count > 0
            && !EndsBefore(date);
        if (change is { IsCancelled: true } || (!onRule && change?.OverrideStartUtc is null))
        {
            return null;
        }

        return Build(date, change);
    }

    /// <summary>Occurrences overlapping [fromUtc, toUtc), overrides applied, cancelled ones left out; at most <see cref="RecurrenceEvaluator.MaxOccurrences"/>.</summary>
    public IReadOnlyList<EventOccurrence> OccurrencesBetween(DateTimeOffset fromUtc, DateTimeOffset toUtc, IEnumerable<EventOverride> overrides)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        var byDate = overrides.ToDictionary(o => o.OriginalInstanceDate);
        var untilDate = SeriesEndUtc is { } end ? EventTime.LocalDate(end, TimeZoneId) : (DateOnly?)null;
        var from = DateOnly.FromDateTime(fromUtc.UtcDateTime).AddDays(-2);
        var to = DateOnly.FromDateTime(toUtc.UtcDateTime).AddDays(1);
        var dates = RecurrenceEvaluator.Dates(Rule.Value, StartDate, untilDate, from, to);

        var result = new List<EventOccurrence>();
        foreach (var date in dates.Where(d => !EndsBefore(d)))
        {
            byDate.TryGetValue(date, out var change);
            if (change is { IsCancelled: true })
            {
                continue;
            }

            result.Add(Build(date, change));
        }

        foreach (var change in byDate.Values.Where(o => !o.IsCancelled && o.OverrideStartUtc is not null && !dates.Contains(o.OriginalInstanceDate)))
        {
            result.Add(Build(change.OriginalInstanceDate, change));
        }

        return result.Where(o => o.EndUtc >= fromUtc && o.StartUtc < toUtc).OrderBy(o => o.StartUtc).Take(RecurrenceEvaluator.MaxOccurrences).ToList();
    }

    /// <summary>The first occurrence of the series (looked for within eight years), or null when the rule yields none.</summary>
    public EventOccurrence? FirstOccurrence() => OccurrencesBetween(SeriesStartUtc, SeriesStartUtc.AddYears(8), []) is [var first, ..] ? first : null;

    /// <summary>Whether a guest sees the series: not private (a series is for the whole household).</summary>
    public bool IsVisibleToGuest => !IsPrivate;

    private bool EndsBefore(DateOnly date) =>
        SeriesEndUtc is { } end && EventTime.ToUtc(date, LocalStartTime, TimeZoneId) > end;

    private EventOccurrence Build(DateOnly date, EventOverride? change)
    {
        var start = EventTime.ToUtc(date, LocalStartTime, TimeZoneId);
        var endDate = IsAllDay || LocalEndTime < LocalStartTime ? date.AddDays(1) : date;
        var end = IsAllDay ? EventTime.ToUtc(endDate, LocalStartTime, TimeZoneId) : EventTime.ToUtc(endDate, LocalEndTime, TimeZoneId);
        if (change?.OverrideStartUtc is { } s)
        {
            end = change.OverrideEndUtc ?? s + (end - start);
            start = s;
        }
        else if (change?.OverrideEndUtc is { } e)
        {
            end = e;
        }

        return new EventOccurrence(
            Id, date, change?.OverrideTitle?.Value ?? Title.Value, Description, Location, TimeZoneId, start, end, IsAllDay,
            (change?.OverrideReminders ?? []).Select(r => r.MinutesBefore).ToList());
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
