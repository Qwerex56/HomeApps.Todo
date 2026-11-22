using System.Globalization;
using System.Text;
using FluentResults;
using Todo.Domain.Common;

namespace Todo.Domain.Events;

/// <summary>
/// Reads the VEVENTs of an iCalendar (RFC 5545) text: UID, SUMMARY, DESCRIPTION, LOCATION, DTSTART, DTEND, RRULE and
/// EXDATE. Cancelled events and single-instance edits (RECURRENCE-ID) are skipped. Everything else is ignored.
/// </summary>
public static class IcsParser
{
    public const int MaxEvents = 5000;

    private const int MaxTitleLength = 200;
    private const int MaxTextLength = 2000;

    public static Result<IReadOnlyList<ExternalEvent>> Parse(string? text)
    {
        if (string.IsNullOrWhiteSpace(text) || !text.Contains("BEGIN:VCALENDAR", StringComparison.OrdinalIgnoreCase))
        {
            return Result.Fail<IReadOnlyList<ExternalEvent>>(PlannerErrors.ParseError);
        }

        var events = new List<ExternalEvent>();
        List<Property>? current = null;
        foreach (var property in Unfold(text).Select(ParseLine).OfType<Property>())
        {
            if (property.Name == "BEGIN" && property.Value.Equals("VEVENT", StringComparison.OrdinalIgnoreCase))
            {
                current = [];
            }
            else if (property.Name == "END" && property.Value.Equals("VEVENT", StringComparison.OrdinalIgnoreCase))
            {
                if (current is not null && Build(current) is { } built)
                {
                    events.Add(built);
                    if (events.Count >= MaxEvents)
                    {
                        break;
                    }
                }

                current = null;
            }
            else
            {
                current?.Add(property);
            }
        }

        return events;
    }

    private static ExternalEvent? Build(List<Property> props)
    {
        if (props.Any(p => p.Name == "RECURRENCE-ID") || props.Any(p => p.Name == "STATUS" && p.Value.Equals("CANCELLED", StringComparison.OrdinalIgnoreCase)))
        {
            return null;
        }

        var start = props.FirstOrDefault(p => p.Name == "DTSTART");
        if (start is null || ParseTime(start) is not { } from)
        {
            return null;
        }

        var end = props.FirstOrDefault(p => p.Name == "DTEND") is { } e && ParseTime(e) is { } parsedEnd && parsedEnd.Instant >= from.Instant
            ? parsedEnd.Instant
            : from.Instant + (from.IsAllDay ? TimeSpan.FromDays(1) : TimeSpan.Zero);

        var title = Unescape(props.FirstOrDefault(p => p.Name == "SUMMARY")?.Value);
        title = string.IsNullOrWhiteSpace(title) ? "(no title)" : Truncate(title.Trim(), MaxTitleLength);
        var excluded = props.Where(p => p.Name == "EXDATE")
            .SelectMany(p => p.Value.Split(',').Select(v => ParseTime(p with { Value = v })))
            .OfType<Time>()
            .Select(t => EventTime.LocalDate(t.Instant, from.Zone))
            .ToList();

        return new ExternalEvent(
            Truncate(props.FirstOrDefault(p => p.Name == "UID")?.Value ?? Guid.NewGuid().ToString(), 255),
            title,
            Text.OrNull(Truncate(Unescape(props.FirstOrDefault(p => p.Name == "DESCRIPTION")?.Value), MaxTextLength)),
            Text.OrNull(Truncate(Unescape(props.FirstOrDefault(p => p.Name == "LOCATION")?.Value), MaxTextLength)),
            from.Zone, from.Instant, end, from.IsAllDay, Text.OrNull(props.FirstOrDefault(p => p.Name == "RRULE")?.Value), excluded);
    }

    private static Time? ParseTime(Property property)
    {
        var value = property.Value.Trim();
        var isDate = property.Parameter("VALUE")?.Equals("DATE", StringComparison.OrdinalIgnoreCase) == true || value.Length == 8;
        if (isDate)
        {
            return DateTime.TryParseExact(value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var day)
                ? new Time(new DateTimeOffset(day, TimeSpan.Zero), "UTC", true)
                : null;
        }

        var utc = value.EndsWith('Z');
        if (!DateTime.TryParseExact(value.TrimEnd('Z'), "yyyyMMdd'T'HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var local))
        {
            return null;
        }

        var zone = "UTC";
        if (!utc && property.Parameter("TZID") is { } tzid && TryFindZone(tzid, out var found))
        {
            zone = found;
        }

        return new Time(new DateTimeOffset(EventTime.ToUtc(DateOnly.FromDateTime(local), TimeOnly.FromDateTime(local), zone).UtcDateTime, TimeSpan.Zero), zone, false);
    }

    private static bool TryFindZone(string id, out string zone)
    {
        zone = id.Trim('"');
        try
        {
            TimeZoneInfo.FindSystemTimeZoneById(zone);
            return true;
        }
        catch (Exception e) when (e is TimeZoneNotFoundException or InvalidTimeZoneException)
        {
            return false;
        }
    }

    private static IEnumerable<string> Unfold(string text)
    {
        var line = new StringBuilder();
        foreach (var raw in text.Split('\n'))
        {
            var row = raw.TrimEnd('\r');
            if (row.Length > 0 && row[0] is ' ' or '\t')
            {
                line.Append(row, 1, row.Length - 1);
                continue;
            }

            if (line.Length > 0)
            {
                yield return line.ToString();
                line.Clear();
            }

            line.Append(row);
        }

        if (line.Length > 0)
        {
            yield return line.ToString();
        }
    }

    private static Property? ParseLine(string line)
    {
        var colon = IndexOutsideQuotes(line, ':');
        if (colon <= 0)
        {
            return null;
        }

        var head = line[..colon].Split(';');
        var parameters = head.Skip(1).Select(p => p.Split('=', 2)).Where(p => p.Length == 2)
            .GroupBy(p => p[0].ToUpperInvariant()).ToDictionary(g => g.Key, g => g.First()[1]);
        return new Property(head[0].ToUpperInvariant(), parameters, line[(colon + 1)..]);
    }

    private static int IndexOutsideQuotes(string line, char c)
    {
        var quoted = false;
        for (var i = 0; i < line.Length; i++)
        {
            if (line[i] == '"')
            {
                quoted = !quoted;
            }
            else if (line[i] == c && !quoted)
            {
                return i;
            }
        }

        return -1;
    }

    private static string? Unescape(string? value) =>
        value?.Replace("\\n", "\n", StringComparison.OrdinalIgnoreCase).Replace("\\,", ",").Replace("\\;", ";").Replace("\\\\", "\\");

    private static string Truncate(string? value, int max) => value is null ? string.Empty : value.Length > max ? value[..max] : value;

    private sealed record Property(string Name, Dictionary<string, string> Parameters, string Value)
    {
        public string? Parameter(string name) => Parameters.GetValueOrDefault(name);
    }

    private sealed record Time(DateTimeOffset Instant, string Zone, bool IsAllDay);
}
