using FluentResults;
using Todo.Domain.Common;

namespace Todo.Domain.Events;

/// <summary>Checks shared by single events and series.</summary>
internal static class EventTime
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
}
