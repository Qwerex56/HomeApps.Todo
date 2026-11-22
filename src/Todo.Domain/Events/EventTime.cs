using FluentResults;
using Todo.Domain.Common;

namespace Todo.Domain.Events;

/// <summary>Checks shared by single events and series.</summary>
public static class EventTime
{
    public static Result CheckZone(string? zone)
    {
        if (string.IsNullOrWhiteSpace(zone))
        {
            return Result.Fail(PlannerErrors.InvalidTimeZone);
        }

        try
        {
            TimeZoneInfo.FindSystemTimeZoneById(zone);
            return Result.Ok();
        }
        catch (Exception e) when (e is TimeZoneNotFoundException or InvalidTimeZoneException)
        {
            return Result.Fail(PlannerErrors.InvalidTimeZone);
        }
    }

    public static TimeOnly LocalTime(DateTimeOffset utc, string zone) =>
        TimeOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(utc.UtcDateTime, TimeZoneInfo.FindSystemTimeZoneById(zone)));

    public static DateOnly LocalDate(DateTimeOffset utc, string zone) =>
        DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(utc.UtcDateTime, TimeZoneInfo.FindSystemTimeZoneById(zone)));

    /// <summary>The instant of a local date and time in the zone; a time skipped by a daylight saving jump moves one hour on.</summary>
    public static DateTimeOffset ToUtc(DateOnly date, TimeOnly time, string zone)
    {
        var tz = TimeZoneInfo.FindSystemTimeZoneById(zone);
        var local = date.ToDateTime(time, DateTimeKind.Unspecified);
        if (tz.IsInvalidTime(local))
        {
            local = local.AddHours(1);
        }

        return new DateTimeOffset(TimeZoneInfo.ConvertTimeToUtc(local, tz), TimeSpan.Zero);
    }
}
