using Todo.Application.Common;
using Todo.Domain.Common;
using Todo.Domain.Events;
using Todo.Infrastructure.Feeds;

namespace Todo.Api.Tests;

public sealed class IcsFeedGeneratorTests
{
    private static readonly DateTimeOffset Now = new(2026, 11, 22, 10, 0, 0, TimeSpan.Zero);

    private static string Ics(string title, string? description = null, bool allDay = false, DateTimeOffset? start = null, DateTimeOffset? end = null, string zone = "UTC")
    {
        var s = start ?? Now;
        var e = CalendarEvent.Create(
            Guid.NewGuid(), new Actor(Guid.NewGuid(), false), title, description, null, zone, s, end ?? s.AddHours(1), allDay, false, [], [], TimeProvider.System);
        return new IcsFeedGenerator().Generate("Cal", [e.Value], [], [], Now);
    }

    [Fact]
    public void Text_IsEscapedAndLinesAreCrlf()
    {
        var ics = Ics("a;b,c\\d", "x\ny");

        Assert.Contains("SUMMARY:a\\;b\\,c\\\\d\r\n", ics, StringComparison.Ordinal);
        Assert.Contains("DESCRIPTION:x\\ny\r\n", ics, StringComparison.Ordinal);
        Assert.DoesNotContain("\n", ics.Replace("\r\n", "", StringComparison.Ordinal), StringComparison.Ordinal);
    }

    [Fact]
    public void LongLines_AreFoldedAt75OctetsWithoutSplittingCharacters()
    {
        var ics = Ics("t", new string('ż', 100));

        var lines = ics.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
        Assert.All(lines, l => Assert.True(System.Text.Encoding.UTF8.GetByteCount(l) <= 75));
        Assert.Contains(lines, l => l.StartsWith(' '));
    }

    [Fact]
    public void AllDay_SingleAndMultiDayGetTheDayAfterAsEnd()
    {
        var day = new DateTimeOffset(2026, 11, 24, 0, 0, 0, TimeSpan.Zero);
        var single = Ics("s", allDay: true, start: day, end: day.AddHours(23));
        var multi = Ics("m", allDay: true, start: day, end: day.AddDays(3));

        Assert.Contains("DTEND;VALUE=DATE:20261125", single, StringComparison.Ordinal);
        Assert.Contains("DTEND;VALUE=DATE:20261127", multi, StringComparison.Ordinal);
    }
}
