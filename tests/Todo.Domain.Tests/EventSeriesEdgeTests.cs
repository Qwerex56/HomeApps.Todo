using Todo.Domain.Common;
using Todo.Domain.Events;

namespace Todo.Domain.Tests;

/// <summary>Edge cases of series occurrences: DST gap, windows cutting an occurrence, all-day occurrences (spec 2.3, 4).</summary>
public sealed class EventSeriesEdgeTests
{
    private readonly FixedTime _time = new(A.Now);
    private readonly Guid _household = Guid.NewGuid();
    private readonly Actor _author = Actor.Member(Guid.NewGuid());

    [Fact]
    public void Daily_AtATimeSkippedByTheSpringJump_StartsAnHourLaterOnThatDay()
    {
        // 2026-03-29 02:00-03:00 does not exist in Europe/Warsaw; 02:30 local becomes 03:30 CEST (01:30 UTC).
        var series = EventSeries.Create(_household, _author, "Leki", null, null, "Europe/Warsaw",
            new DateTimeOffset(2026, 3, 28, 1, 30, 0, TimeSpan.Zero), new DateTimeOffset(2026, 3, 28, 2, 30, 0, TimeSpan.Zero),
            false, false, "FREQ=DAILY", null, _time).Value;

        var found = series.OccurrencesBetween(new DateTimeOffset(2026, 3, 28, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2026, 3, 31, 0, 0, 0, TimeSpan.Zero), []);

        Assert.Equal(
            [new DateTimeOffset(2026, 3, 28, 1, 30, 0, TimeSpan.Zero), new DateTimeOffset(2026, 3, 29, 1, 30, 0, TimeSpan.Zero),
             new DateTimeOffset(2026, 3, 30, 0, 30, 0, TimeSpan.Zero)],
            found.Select(o => o.StartUtc));
    }

    [Fact]
    public void Window_StartingDuringAnOccurrence_StillReturnsIt()
    {
        var series = EventSeries.Create(_household, _author, "Obiad", null, null, "Europe/Warsaw",
            new DateTimeOffset(2026, 10, 20, 14, 0, 0, TimeSpan.Zero), new DateTimeOffset(2026, 10, 20, 15, 0, 0, TimeSpan.Zero),
            false, false, "FREQ=DAILY", null, _time).Value;

        var found = series.OccurrencesBetween(new DateTimeOffset(2026, 10, 20, 14, 30, 0, TimeSpan.Zero), new DateTimeOffset(2026, 10, 20, 14, 45, 0, TimeSpan.Zero), []);

        Assert.Equal([new DateTimeOffset(2026, 10, 20, 14, 0, 0, TimeSpan.Zero)], found.Select(o => o.StartUtc));
    }

    [Fact]
    public void Window_EndingExactlyAtAnOccurrenceStart_DoesNotReturnIt()
    {
        // The series starts 2026-10-20 14:00 UTC, so the window [Oct 19, Oct 20 14:00) holds no occurrence.
        var series = EventSeries.Create(_household, _author, "Obiad", null, null, "Europe/Warsaw",
            new DateTimeOffset(2026, 10, 20, 14, 0, 0, TimeSpan.Zero), new DateTimeOffset(2026, 10, 20, 15, 0, 0, TimeSpan.Zero),
            false, false, "FREQ=DAILY", null, _time).Value;

        var found = series.OccurrencesBetween(new DateTimeOffset(2026, 10, 19, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2026, 10, 20, 14, 0, 0, TimeSpan.Zero), []);

        Assert.Empty(found);
    }

    [Fact]
    public void AllDay_Occurrences_SpanOneLocalDayEach()
    {
        // All-day from 2026-05-01 in Warsaw (local midnight is 22:00 UTC the day before).
        var series = EventSeries.Create(_household, _author, "Urlop", null, null, "Europe/Warsaw",
            new DateTimeOffset(2026, 4, 30, 22, 0, 0, TimeSpan.Zero), new DateTimeOffset(2026, 5, 1, 22, 0, 0, TimeSpan.Zero),
            true, false, "FREQ=DAILY;COUNT=2", null, _time).Value;

        var found = series.OccurrencesBetween(new DateTimeOffset(2026, 4, 30, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2026, 5, 10, 0, 0, 0, TimeSpan.Zero), []);

        Assert.All(found, o => Assert.Equal(TimeSpan.FromDays(1), o.EndUtc - o.StartUtc));
        Assert.All(found, o => Assert.True(o.IsAllDay));
        Assert.Equal(
            [new DateTimeOffset(2026, 4, 30, 22, 0, 0, TimeSpan.Zero), new DateTimeOffset(2026, 5, 1, 22, 0, 0, TimeSpan.Zero)],
            found.Select(o => o.StartUtc));
    }

    [Fact]
    public void Rule_ThatIsNotSupported_IsRefusedWhenTheSeriesIsCreated()
    {
        var result = EventSeries.Create(_household, _author, "Obiad", null, null, "Europe/Warsaw",
            new DateTimeOffset(2026, 10, 20, 14, 0, 0, TimeSpan.Zero), new DateTimeOffset(2026, 10, 20, 15, 0, 0, TimeSpan.Zero),
            false, false, "FREQ=HOURLY", null, _time);

        Assert.Equal("planner.invalid_rule", result.Code());
    }
}
