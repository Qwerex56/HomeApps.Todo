using System.Globalization;
using System.Text;
using Todo.Application.Abstractions;
using Todo.Application.Common;
using Todo.Domain.Events;

namespace Todo.Infrastructure.Feeds;

/// <summary>Writes RFC 5545 text: CRLF lines folded at 75 octets, text values escaped, times in UTC.</summary>
internal sealed class IcsFeedGenerator : ICalendarFeedGenerator
{
    public string Generate(
        string name, IReadOnlyList<CalendarEvent> events, IReadOnlyList<(EventSeries Series, EventOccurrence Occurrence)> occurrences,
        IReadOnlyList<TaskView> tasks, DateTimeOffset now)
    {
        var lines = new List<string> { "BEGIN:VCALENDAR", "VERSION:2.0", "PRODID:-//HomeApps//Todo//EN", "CALSCALE:GREGORIAN", $"X-WR-CALNAME:{Escape(name)}" };
        var stamp = Utc(now);

        foreach (var e in events)
        {
            AddEvent(lines, $"{e.Id}@homeapps-todo", stamp, e.Title.Value, e.Description, e.Location, e.StartUtc, e.EndUtc, e.IsAllDay, e.TimeZoneId);
        }

        foreach (var (series, o) in occurrences)
        {
            AddEvent(lines, $"{series.Id}-{o.OriginalDate:yyyyMMdd}@homeapps-todo", stamp, o.Title, o.Description, o.Location, o.StartUtc, o.EndUtc, o.IsAllDay, o.TimeZoneId);
        }

        foreach (var t in tasks.Where(t => t.DueDate is not null))
        {
            lines.Add("BEGIN:VEVENT");
            lines.Add($"UID:task-{t.Id}@homeapps-todo");
            lines.Add($"DTSTAMP:{stamp}");
            if (t.DueTime is { } time)
            {
                // A task's due time has no zone: floating local time, one hour long.
                var startLocal = t.DueDate!.Value.ToDateTime(time);
                lines.Add($"DTSTART:{startLocal:yyyyMMdd'T'HHmmss}");
                lines.Add($"DTEND:{startLocal.AddHours(1):yyyyMMdd'T'HHmmss}");
            }
            else
            {
                lines.Add($"DTSTART;VALUE=DATE:{t.DueDate!.Value:yyyyMMdd}");
                lines.Add($"DTEND;VALUE=DATE:{t.DueDate.Value.AddDays(1):yyyyMMdd}");
            }

            lines.Add($"SUMMARY:{Escape(t.Title)}");
            if (t.Notes is not null)
            {
                lines.Add($"DESCRIPTION:{Escape(t.Notes)}");
            }

            lines.Add("END:VEVENT");
        }

        lines.Add("END:VCALENDAR");
        var sb = new StringBuilder();
        foreach (var line in lines)
        {
            sb.Append(Fold(line)).Append("\r\n");
        }

        return sb.ToString();
    }

    private static void AddEvent(
        List<string> lines, string uid, string stamp, string title, string? description, string? location,
        DateTimeOffset start, DateTimeOffset end, bool allDay, string timeZoneId)
    {
        lines.Add("BEGIN:VEVENT");
        lines.Add($"UID:{uid}");
        lines.Add($"DTSTAMP:{stamp}");
        if (allDay)
        {
            var (s, e) = LocalDates(start, end, timeZoneId);
            lines.Add($"DTSTART;VALUE=DATE:{s:yyyyMMdd}");
            lines.Add($"DTEND;VALUE=DATE:{e:yyyyMMdd}");
        }
        else
        {
            lines.Add($"DTSTART:{Utc(start)}");
            lines.Add($"DTEND:{Utc(end)}");
        }

        lines.Add($"SUMMARY:{Escape(title)}");
        if (!string.IsNullOrEmpty(description))
        {
            lines.Add($"DESCRIPTION:{Escape(description)}");
        }

        if (!string.IsNullOrEmpty(location))
        {
            lines.Add($"LOCATION:{Escape(location)}");
        }

        lines.Add("END:VEVENT");
    }

    /// <summary>All-day events are whole local days; DTEND is the day after the last one. An unknown zone counts as UTC.</summary>
    private static (DateOnly Start, DateOnly EndExclusive) LocalDates(DateTimeOffset start, DateTimeOffset end, string timeZoneId)
    {
        TimeZoneInfo zone;
        try
        {
            zone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }
        catch (Exception ex) when (ex is TimeZoneNotFoundException or InvalidTimeZoneException)
        {
            zone = TimeZoneInfo.Utc;
        }

        var s = DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(start, zone).DateTime);
        var endLocal = TimeZoneInfo.ConvertTime(end, zone).DateTime;
        var e = DateOnly.FromDateTime(endLocal);

        // The end is either the next midnight (exclusive) or any time on the last day (inclusive).
        if (endLocal.TimeOfDay != TimeSpan.Zero || e <= s)
        {
            e = e.AddDays(1);
        }

        return (s, e);
    }

    private static string Utc(DateTimeOffset value) => value.UtcDateTime.ToString("yyyyMMdd'T'HHmmss'Z'", CultureInfo.InvariantCulture);

    private static string Escape(string value) =>
        value.Replace("\\", "\\\\", StringComparison.Ordinal).Replace(";", "\\;", StringComparison.Ordinal).Replace(",", "\\,", StringComparison.Ordinal)
            .Replace("\r\n", "\\n", StringComparison.Ordinal).Replace("\n", "\\n", StringComparison.Ordinal).Replace("\r", "\\n", StringComparison.Ordinal);

    private static string Fold(string line)
    {
        if (Encoding.UTF8.GetByteCount(line) <= 75)
        {
            return line;
        }

        var sb = new StringBuilder();
        var octets = 0;
        foreach (var rune in line.EnumerateRunes())
        {
            var size = rune.Utf8SequenceLength;
            if (octets + size > 75)
            {
                sb.Append("\r\n ");
                octets = 1;
            }

            sb.Append(rune.ToString());
            octets += size;
        }

        return sb.ToString();
    }
}
