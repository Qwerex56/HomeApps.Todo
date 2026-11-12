using System.Globalization;
using FluentResults;
using HomeApps.BuildingBlocks.Application;

namespace Todo.Api.Mcp;

/// <summary>A due date as a tool gets it: <c>2026-11-13</c> or <c>2026-11-13 18:00</c>. The model turns "jutro" into a date.</summary>
internal static class Due
{
    private static readonly string[] Formats = ["yyyy-MM-dd", "yyyy-MM-dd HH:mm", "yyyy-MM-ddTHH:mm", "yyyy-MM-ddTHH:mm:ss"];

    public static Result<(DateOnly? Date, TimeOnly? Time)> Parse(string? due)
    {
        if (string.IsNullOrWhiteSpace(due))
        {
            return Result.Ok<(DateOnly?, TimeOnly?)>((null, null));
        }

        if (!DateTime.TryParseExact(due.Trim(), Formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var value))
        {
            return Result.Fail(ValidationErrors.ForField("due", "Use a date like 2026-11-13, optionally with a time: 2026-11-13 18:00."));
        }

        var time = due.Trim().Length > 10 ? TimeOnly.FromDateTime(value) : (TimeOnly?)null;
        return Result.Ok<(DateOnly?, TimeOnly?)>((DateOnly.FromDateTime(value), time));
    }

    public static string Describe(DateOnly? date, TimeOnly? time) => date is not { } day
        ? string.Empty
        : time is { } at
            ? string.Create(CultureInfo.InvariantCulture, $"{day:yyyy-MM-dd} {at:HH\\:mm}")
            : day.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
}
