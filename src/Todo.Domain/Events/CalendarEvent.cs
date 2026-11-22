using FluentResults;
using Todo.Domain.Common;

namespace Todo.Domain.Events;

/// <summary>
/// One calendar event of a household: a single event, or an occurrence stored on its own when it belongs to a series.
/// Local time and IANA zone are the truth; <see cref="StartUtc"/> and <see cref="EndUtc"/> are denormalised for queries.
/// A private event is visible to its author only.
/// </summary>
public sealed class CalendarEvent
{
    private CalendarEvent() { }

    private CalendarEvent(Guid id, Guid householdId, Guid createdBy, DateTimeOffset now)
    {
        Id = id;
        HouseholdId = householdId;
        CreatedBy = createdBy;
        CreatedAt = UpdatedAt = now;
    }

    public Guid Id { get; private set; }

    public Guid HouseholdId { get; private set; }

    public Guid? SeriesId { get; private set; }

    public Guid CreatedBy { get; private set; }

    public EventTitle Title { get; private set; } = null!;

    public string? Description { get; private set; }

    public string? Location { get; private set; }

    public string TimeZoneId { get; private set; } = null!;

    public DateTimeOffset StartUtc { get; private set; }

    public DateTimeOffset EndUtc { get; private set; }

    public TimeOnly LocalStartTime { get; private set; }

    public TimeOnly LocalEndTime { get; private set; }

    public bool IsAllDay { get; private set; }

    public bool IsPrivate { get; private set; }

    /// <summary>Empty means the whole household.</summary>
    public List<Guid> Attendees { get; private set; } = [];

    public List<EventReminder> Reminders { get; private set; } = [];

    /// <summary>Set on a private event while its author is out of the household.</summary>
    public DateTimeOffset? OwnerLeftAt { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public bool IsForWholeHousehold => Attendees.Count == 0;

    public static Result<CalendarEvent> Create(
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
        IEnumerable<Guid>? attendees,
        IEnumerable<int>? reminders,
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
        var rems = EventReminder.CreateMany(reminders);
        var range = endUtc < startUtc ? Result.Fail(PlannerErrors.InvalidDateRange) : Result.Ok();
        var check = Result.Merge(validTitle, zone, rems, range);
        if (check.IsFailed)
        {
            return check.ToResult<CalendarEvent>();
        }

        var now = time.GetUtcNow();
        var created = new CalendarEvent(Guid.CreateVersion7(now), householdId, actor.UserId, now);
        created.Apply(validTitle.Value, description, location, timeZoneId!, startUtc, endUtc, isAllDay, isPrivate, attendees, rems.Value);
        return created;
    }

    /// <summary>Private events are for their author only (also not for admins); otherwise any member sees it.</summary>
    public bool CanSee(Actor actor)
    {
        ArgumentNullException.ThrowIfNull(actor);
        return !IsPrivate || (actor.UserId == CreatedBy && OwnerLeftAt is null);
    }

    /// <summary>A guest sees only events that attend them or are for the whole household, never private ones.</summary>
    public bool IsVisibleToGuest(Guid userId) => !IsPrivate && (IsForWholeHousehold || Attendees.Contains(userId));

    /// <summary>The author, an attendee, or anyone when the event is for the whole household. Private: the author alone.</summary>
    public Result CanEdit(Actor actor)
    {
        if (!CanSee(actor))
        {
            return Result.Fail(PlannerErrors.EventNotFound);
        }

        return actor.UserId == CreatedBy || IsForWholeHousehold || Attendees.Contains(actor.UserId)
            ? Result.Ok()
            : Result.Fail(PlannerErrors.CannotEditEvent);
    }

    /// <summary>The author or a household admin; a private event only its author.</summary>
    public Result CanDelete(Actor actor)
    {
        if (!CanSee(actor))
        {
            return Result.Fail(PlannerErrors.EventNotFound);
        }

        return actor.UserId == CreatedBy || (!IsPrivate && actor.Manages) ? Result.Ok() : Result.Fail(PlannerErrors.CannotDeleteEvent);
    }

    public Result Edit(
        Actor actor,
        string? title,
        string? description,
        string? location,
        string? timeZoneId,
        DateTimeOffset startUtc,
        DateTimeOffset endUtc,
        bool isAllDay,
        bool isPrivate,
        IEnumerable<Guid>? attendees,
        IEnumerable<int>? reminders,
        TimeProvider time)
    {
        ArgumentNullException.ThrowIfNull(time);
        var allowed = CanEdit(actor);
        if (allowed.IsFailed)
        {
            return allowed;
        }

        if (isPrivate != IsPrivate && actor.UserId != CreatedBy)
        {
            return Result.Fail(PlannerErrors.CannotEditEvent);
        }

        var validTitle = EventTitle.Create(title);
        var zone = EventTime.CheckZone(timeZoneId);
        var rems = EventReminder.CreateMany(reminders);
        var range = endUtc < startUtc ? Result.Fail(PlannerErrors.InvalidDateRange) : Result.Ok();
        var check = Result.Merge(validTitle, zone, rems, range);
        if (check.IsFailed)
        {
            return check;
        }

        Apply(validTitle.Value, description, location, timeZoneId!, startUtc, endUtc, isAllDay, isPrivate, attendees, rems.Value);
        UpdatedAt = time.GetUtcNow();
        return Result.Ok();
    }

    /// <summary>Links the event to the series it is an occurrence of.</summary>
    public void BelongTo(Guid seriesId) => SeriesId = seriesId;

    /// <summary>A member left the household: they no longer attend, and their private event is hidden.</summary>
    public void MemberLeft(Guid userId, TimeProvider time)
    {
        ArgumentNullException.ThrowIfNull(time);
        var now = time.GetUtcNow();
        if (Attendees.Remove(userId))
        {
            if (Attendees.Count == 0 && CreatedBy != userId)
            {
                Attendees.Add(CreatedBy);
            }

            UpdatedAt = now;
        }

        if (IsPrivate && CreatedBy == userId && OwnerLeftAt is null)
        {
            OwnerLeftAt = UpdatedAt = now;
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

    private void Apply(
        EventTitle title, string? description, string? location, string zone, DateTimeOffset startUtc, DateTimeOffset endUtc,
        bool isAllDay, bool isPrivate, IEnumerable<Guid>? attendees, List<EventReminder> reminders)
    {
        Title = title;
        Description = Text.OrNull(description);
        Location = Text.OrNull(location);
        TimeZoneId = zone;
        StartUtc = startUtc.ToUniversalTime();
        EndUtc = endUtc.ToUniversalTime();
        LocalStartTime = EventTime.LocalTime(StartUtc, zone);
        LocalEndTime = EventTime.LocalTime(EndUtc, zone);
        IsAllDay = isAllDay;
        IsPrivate = isPrivate;
        Attendees = isPrivate ? [] : (attendees ?? []).Where(a => a != Guid.Empty).Distinct().ToList();
        Reminders = reminders;
    }
}
