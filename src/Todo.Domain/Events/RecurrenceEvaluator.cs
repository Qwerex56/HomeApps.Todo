namespace Todo.Domain.Events;

/// <summary>
/// Expands an RRULE into local dates (ADR 3.7). Supports FREQ DAILY/WEEKLY/MONTHLY/YEARLY with INTERVAL, COUNT, UNTIL,
/// BYDAY (with an ordinal in MONTHLY/YEARLY), BYMONTHDAY and BYMONTH. Never returns more than <see cref="MaxOccurrences"/> dates.
/// </summary>
public static class RecurrenceEvaluator
{
    public const int MaxOccurrences = 1000;

    private const int MaxPeriods = 100_000;
    private const int MaxInterval = 1000;

    private static readonly string[] Keys = ["FREQ", "INTERVAL", "COUNT", "UNTIL", "BYDAY", "BYMONTHDAY", "BYMONTH", "WKST"];

    private static readonly Dictionary<string, DayOfWeek> Days = new()
    {
        ["MO"] = DayOfWeek.Monday, ["TU"] = DayOfWeek.Tuesday, ["WE"] = DayOfWeek.Wednesday, ["TH"] = DayOfWeek.Thursday,
        ["FR"] = DayOfWeek.Friday, ["SA"] = DayOfWeek.Saturday, ["SU"] = DayOfWeek.Sunday,
    };

    /// <summary>True when the rule only uses what this evaluator supports.</summary>
    public static bool IsSupported(string rule) => TryParse(rule, out _);

    /// <summary>Local dates of the series inside [from, to], in order; <paramref name="start"/> is the first date; <paramref name="until"/> the last allowed.</summary>
    public static IReadOnlyList<DateOnly> Dates(string rule, DateOnly start, DateOnly? until, DateOnly from, DateOnly to)
    {
        if (!TryParse(rule, out var p))
        {
            return [];
        }

        var last = until is { } u && u < to ? u : to;
        if (p.Until is { } pu && pu < last)
        {
            last = pu;
        }

        var found = new List<DateOnly>();
        var emitted = 0;
        for (var k = 0; k < MaxPeriods; k++)
        {
            var periodStart = PeriodStart(p, start, k);
            if (periodStart is null || periodStart > last)
            {
                break;
            }

            foreach (var date in Candidates(p, start, periodStart.Value).Where(d => d >= start && d <= last).Order())
            {
                if (p.Count is { } c && emitted >= c)
                {
                    return found;
                }

                emitted++;
                if (date >= from)
                {
                    found.Add(date);
                    if (found.Count >= MaxOccurrences)
                    {
                        return found;
                    }
                }
            }

            if (p.Count is { } count && emitted >= count)
            {
                break;
            }
        }

        return found;
    }

    private static DateOnly? PeriodStart(Parsed p, DateOnly start, int k)
    {
        try
        {
            var n = k * p.Interval;
            return p.Freq switch
            {
                "DAILY" => start.AddDays(n),
                "WEEKLY" => start.AddDays(-(((int)start.DayOfWeek + 6) % 7)).AddDays(7 * n),
                "MONTHLY" => new DateOnly(start.Year, start.Month, 1).AddMonths(n),
                _ => new DateOnly(start.Year, 1, 1).AddYears(n),
            };
        }
        catch (ArgumentOutOfRangeException)
        {
            return null;
        }
    }

    private static IEnumerable<DateOnly> Candidates(Parsed p, DateOnly start, DateOnly periodStart)
    {
        switch (p.Freq)
        {
            case "DAILY":
                if (Allowed(p, periodStart))
                {
                    yield return periodStart;
                }

                break;
            case "WEEKLY":
                var days = p.ByDay.Count == 0 ? new List<DayOfWeek> { start.DayOfWeek } : p.ByDay.Select(d => d.Day).Distinct().ToList();
                foreach (var day in days)
                {
                    var date = periodStart.AddDays(((int)day + 6) % 7);
                    if (Allowed(p, date))
                    {
                        yield return date;
                    }
                }

                break;
            case "MONTHLY":
                foreach (var d in InMonth(p, start, periodStart.Year, periodStart.Month).Where(d => Allowed(p, d)))
                {
                    yield return d;
                }

                break;
            default:
                if (p.ByMonth.Count == 0 && p.ByMonthDay.Count == 0 && p.ByDay.Count > 0)
                {
                    foreach (var d in p.ByDay.SelectMany(b => OfWeekdayInYear(periodStart.Year, b)))
                    {
                        yield return d;
                    }

                    break;
                }

                var months = p.ByMonth.Count == 0 ? new List<int> { start.Month } : p.ByMonth;
                foreach (var m in months)
                {
                    foreach (var d in InMonth(p, start, periodStart.Year, m))
                    {
                        yield return d;
                    }
                }

                break;
        }
    }

    private static bool Allowed(Parsed p, DateOnly date) =>
        (p.ByMonth.Count == 0 || p.ByMonth.Contains(date.Month))
        && (p.Freq is not "DAILY" || p.ByDay.Count == 0 || p.ByDay.Any(d => d.Day == date.DayOfWeek));

    private static IEnumerable<DateOnly> InMonth(Parsed p, DateOnly start, int year, int month)
    {
        var length = DateTime.DaysInMonth(year, month);
        if (p.ByMonthDay.Count > 0)
        {
            return p.ByMonthDay.Select(d => d > 0 ? d : length + 1 + d).Where(d => d >= 1 && d <= length)
                .Select(d => new DateOnly(year, month, d)).Where(d => p.ByDay.Count == 0 || p.ByDay.Any(b => b.Day == d.DayOfWeek));
        }

        if (p.ByDay.Count > 0)
        {
            return p.ByDay.SelectMany(b => OfWeekday(year, month, length, b));
        }

        return start.Day <= length ? [new DateOnly(year, month, start.Day)] : [];
    }

    private static List<DateOnly> OfWeekday(int year, int month, int length, (int? Ordinal, DayOfWeek Day) b)
    {
        var all = Enumerable.Range(1, length).Select(d => new DateOnly(year, month, d)).Where(d => d.DayOfWeek == b.Day).ToList();
        if (b.Ordinal is not { } n)
        {
            return all;
        }

        var index = n > 0 ? n - 1 : all.Count + n;
        return index >= 0 && index < all.Count ? [all[index]] : [];
    }

    private static List<DateOnly> OfWeekdayInYear(int year, (int? Ordinal, DayOfWeek Day) b)
    {
        var first = new DateOnly(year, 1, 1);
        var all = Enumerable.Range(0, DateTime.IsLeapYear(year) ? 366 : 365).Select(first.AddDays).Where(d => d.DayOfWeek == b.Day).ToList();
        if (b.Ordinal is not { } n)
        {
            return all;
        }

        var index = n > 0 ? n - 1 : all.Count + n;
        return index >= 0 && index < all.Count ? [all[index]] : [];
    }

    private sealed record Parsed(
        string Freq, int Interval, int? Count, DateOnly? Until, List<(int? Ordinal, DayOfWeek Day)> ByDay, List<int> ByMonthDay, List<int> ByMonth);

    private static bool TryParse(string rule, out Parsed parsed)
    {
        parsed = null!;
        var parts = new Dictionary<string, string>();
        foreach (var part in rule.Split(';', StringSplitOptions.RemoveEmptyEntries))
        {
            var kv = part.Split('=', 2);
            if (kv.Length != 2 || !Keys.Contains(kv[0]) || !parts.TryAdd(kv[0], kv[1]))
            {
                return false;
            }
        }

        if (!parts.TryGetValue("FREQ", out var freq) || freq is not ("DAILY" or "WEEKLY" or "MONTHLY" or "YEARLY"))
        {
            return false;
        }

        if (parts.TryGetValue("WKST", out var wkst) && wkst != "MO")
        {
            return false;
        }

        if (freq is "DAILY" or "WEEKLY" && parts.ContainsKey("BYMONTHDAY"))
        {
            return false;
        }

        var interval = 1;
        if (parts.TryGetValue("INTERVAL", out var i) && (!int.TryParse(i, out interval) || interval is < 1 or > MaxInterval))
        {
            return false;
        }

        int? count = null;
        if (parts.TryGetValue("COUNT", out var c))
        {
            if (!int.TryParse(c, out var n) || n < 1)
            {
                return false;
            }

            count = n;
        }

        DateOnly? until = null;
        if (parts.TryGetValue("UNTIL", out var u))
        {
            if (u.Length < 8 || !DateOnly.TryParseExact(u[..8], "yyyyMMdd", out var d))
            {
                return false;
            }

            until = d;
        }

        var byDay = new List<(int?, DayOfWeek)>();
        foreach (var item in parts.GetValueOrDefault("BYDAY", "").Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            var code = item[^Math.Min(2, item.Length)..];
            var ordinal = item.Length > 2 ? item[..^2] : null;
            int? ord = null;
            if (ordinal is not null)
            {
                if (!int.TryParse(ordinal, out var o) || o == 0 || o is < -5 or > 5 || freq is "DAILY" or "WEEKLY")
                {
                    return false;
                }

                ord = o;
            }

            if (!Days.TryGetValue(code, out var day))
            {
                return false;
            }

            byDay.Add((ord, day));
        }

        if (!TryInts(parts.GetValueOrDefault("BYMONTHDAY", ""), -31, 31, out var byMonthDay)
            || !TryInts(parts.GetValueOrDefault("BYMONTH", ""), 1, 12, out var byMonth))
        {
            return false;
        }

        parsed = new Parsed(freq, interval, count, until, byDay, byMonthDay, byMonth);
        return true;
    }

    private static bool TryInts(string raw, int min, int max, out List<int> values)
    {
        values = [];
        foreach (var item in raw.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            if (!int.TryParse(item, out var n) || n < min || n > max || n == 0)
            {
                return false;
            }

            values.Add(n);
        }

        return true;
    }
}
