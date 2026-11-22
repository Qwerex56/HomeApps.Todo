using FluentResults;
using HomeApps.BuildingBlocks.Domain.Errors;

namespace Todo.Domain.Common;

public static class PlannerErrors
{
    public static Error InvalidDateRange => new AppError("planner.invalid_date_range",
        "The end must not be before the start.", ErrorKind.Validation);

    public static Error EmptyTitle => new AppError("planner.empty_title",
        "The event title must have 1–200 characters and no control characters.", ErrorKind.Validation);

    public static Error InvalidScope => new AppError("planner.invalid_scope",
        "The scope must be ThisInstance, ThisAndFuture or All.", ErrorKind.Validation);

    public static Error InvalidRule => new AppError("planner.invalid_rule",
        "The recurrence rule is not a valid RRULE.", ErrorKind.Validation);

    public static Error InvalidTimeZone => new AppError("planner.invalid_time_zone",
        "The time zone is not a known IANA zone.", ErrorKind.Validation);

    public static Error InvalidReminder => new AppError("planner.invalid_reminder",
        "A reminder is 0 minutes to 4 weeks before the start.", ErrorKind.Validation);

    public static Error InvalidImport => new AppError("planner.invalid_import",
        "The import needs a URL and a name of 1–100 characters.", ErrorKind.Validation);

    public static Error EventNotFound => new AppError("planner.event_not_found",
        "The event does not exist.", ErrorKind.NotFound);

    public static Error CannotEditEvent => new AppError("planner.cannot_edit_event",
        "Only the author, an attendee or, for household events, a member edits the event.", ErrorKind.Forbidden);

    public static Error CannotEditPrivate => new AppError("planner.cannot_edit_private",
        "Only the author edits a private event.", ErrorKind.Forbidden);

    public static Error CannotDeleteEvent => new AppError("planner.cannot_delete_event",
        "Only the author or a household admin deletes a household event.", ErrorKind.Forbidden);

    public static Error SeriesNotFound => new AppError("planner.series_not_found",
        "The recurring event does not exist.", ErrorKind.NotFound);

    public static Error SubscriptionNotFound => new AppError("planner.subscription_not_found",
        "The subscription does not exist.", ErrorKind.NotFound);

    public static Error InvalidUrl => new AppError("planner.invalid_url",
        "The calendar address must be an http(s) URL of up to 2048 characters.", ErrorKind.Validation);

    public static Error SsrfRejected => new AppError("planner.ssrf_rejected",
        "The address points to a resource inside the server's network, not to an external calendar.", ErrorKind.Validation);

    public static Error ParseError => new AppError("planner.parse_error",
        "The address did not return a valid iCalendar file.", ErrorKind.Validation);

    public static Error SyncFailed => new AppError("planner.sync_failed",
        "The calendar could not be downloaded.", ErrorKind.Validation);

    public static Error CalendarNotFound => new AppError("planner.calendar_not_found",
        "The imported calendar does not exist.", ErrorKind.NotFound);

    public static Error NotOwner => new AppError("planner.not_owner",
        "Only the person who imported the calendar removes it.", ErrorKind.Forbidden);
}
