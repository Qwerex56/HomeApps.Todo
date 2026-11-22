using Todo.Domain.Events;

namespace Todo.Domain.Tests;

public sealed class IcsParserTests
{
    private static string Wrap(string body) => "BEGIN:VCALENDAR\r\nVERSION:2.0\r\n" + body + "END:VCALENDAR\r\n";

    [Theory]
    [InlineData("")]
    [InlineData("hello")]
    [InlineData("<html>BEGIN</html>")]
    public void Parse_RejectsWhatIsNotACalendar(string text) => Assert.True(IcsParser.Parse(text).IsFailed);

    [Fact]
    public void Parse_ReadsUtcEvent()
    {
        var result = IcsParser.Parse(Wrap("BEGIN:VEVENT\r\nUID:a1\r\nSUMMARY:Dentist\\, 3rd floor\r\nDTSTART:20260310T090000Z\r\nDTEND:20260310T100000Z\r\nLOCATION:Main St\r\nEND:VEVENT\r\n"));

        var e = Assert.Single(result.Value);
        Assert.Equal("a1", e.Uid);
        Assert.Equal("Dentist, 3rd floor", e.Title);
        Assert.Equal("Main St", e.Location);
        Assert.Equal(new DateTimeOffset(2026, 3, 10, 9, 0, 0, TimeSpan.Zero), e.StartUtc);
        Assert.Equal(TimeSpan.FromHours(1), e.EndUtc - e.StartUtc);
        Assert.False(e.IsAllDay);
    }

    [Fact]
    public void Parse_UnfoldsLongLines()
    {
        var result = IcsParser.Parse(Wrap("BEGIN:VEVENT\r\nUID:a\r\nSUMMARY:Very long\r\n  title\r\nDTSTART:20260310T090000Z\r\nEND:VEVENT\r\n"));

        Assert.Equal("Very long title", result.Value[0].Title);
    }

    [Fact]
    public void Parse_AllDayEndIsExclusiveDate()
    {
        var e = IcsParser.Parse(Wrap("BEGIN:VEVENT\r\nUID:a\r\nSUMMARY:Holiday\r\nDTSTART;VALUE=DATE:20260501\r\nDTEND;VALUE=DATE:20260502\r\nEND:VEVENT\r\n")).Value[0];

        Assert.True(e.IsAllDay);
        Assert.Equal(new DateTimeOffset(2026, 5, 1, 0, 0, 0, TimeSpan.Zero), e.StartUtc);
        Assert.Equal(new DateTimeOffset(2026, 5, 2, 0, 0, 0, TimeSpan.Zero), e.EndUtc);
    }

    [Fact]
    public void Parse_AllDayWithoutEndLastsADay()
    {
        var e = IcsParser.Parse(Wrap("BEGIN:VEVENT\r\nUID:a\r\nDTSTART;VALUE=DATE:20260501\r\nEND:VEVENT\r\n")).Value[0];

        Assert.Equal(TimeSpan.FromDays(1), e.EndUtc - e.StartUtc);
        Assert.Equal("(no title)", e.Title);
    }

    [Fact]
    public void Parse_UsesTzid()
    {
        var e = IcsParser.Parse(Wrap("BEGIN:VEVENT\r\nUID:a\r\nSUMMARY:x\r\nDTSTART;TZID=Europe/Warsaw:20260115T160000\r\nDTEND;TZID=Europe/Warsaw:20260115T170000\r\nEND:VEVENT\r\n")).Value[0];

        Assert.Equal("Europe/Warsaw", e.TimeZoneId);
        Assert.Equal(new DateTimeOffset(2026, 1, 15, 15, 0, 0, TimeSpan.Zero), e.StartUtc);
    }

    [Fact]
    public void Parse_SkipsCancelledInstanceEditsAndBrokenEvents()
    {
        var text = Wrap(
            "BEGIN:VEVENT\r\nUID:1\r\nSTATUS:CANCELLED\r\nDTSTART:20260310T090000Z\r\nEND:VEVENT\r\n" +
            "BEGIN:VEVENT\r\nUID:2\r\nRECURRENCE-ID:20260310T090000Z\r\nDTSTART:20260310T090000Z\r\nEND:VEVENT\r\n" +
            "BEGIN:VEVENT\r\nUID:3\r\nSUMMARY:no start\r\nEND:VEVENT\r\n" +
            "BEGIN:VEVENT\r\nUID:4\r\nSUMMARY:ok\r\nDTSTART:20260310T090000Z\r\nEND:VEVENT\r\n");

        Assert.Equal("4", Assert.Single(IcsParser.Parse(text).Value).Uid);
    }

    [Fact]
    public void Occurrences_ExpandWeeklyRuleKeepingLocalTimeAcrossDst()
    {
        var e = IcsParser.Parse(Wrap(
            "BEGIN:VEVENT\r\nUID:a\r\nSUMMARY:Class\r\nDTSTART;TZID=Europe/Warsaw:20260320T160000\r\nDTEND;TZID=Europe/Warsaw:20260320T170000\r\n" +
            "RRULE:FREQ=WEEKLY\r\nEXDATE;TZID=Europe/Warsaw:20260403T160000\r\nEND:VEVENT\r\n")).Value[0];

        var found = e.OccurrencesBetween(new DateTimeOffset(2026, 3, 19, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2026, 4, 12, 0, 0, 0, TimeSpan.Zero));

        // 20 Mar (winter, 15:00Z), 27 Mar (winter), 3 Apr excluded, 10 Apr (summer, 14:00Z).
        Assert.Equal(
            [new DateTimeOffset(2026, 3, 20, 15, 0, 0, TimeSpan.Zero), new DateTimeOffset(2026, 3, 27, 15, 0, 0, TimeSpan.Zero), new DateTimeOffset(2026, 4, 10, 14, 0, 0, TimeSpan.Zero)],
            found.Select(o => o.StartUtc));
    }

    [Fact]
    public void Occurrences_OfSingleEventOnlyInsideWindow()
    {
        var e = IcsParser.Parse(Wrap("BEGIN:VEVENT\r\nUID:a\r\nSUMMARY:x\r\nDTSTART:20260310T090000Z\r\nDTEND:20260310T100000Z\r\nEND:VEVENT\r\n")).Value[0];

        Assert.Single(e.OccurrencesBetween(new DateTimeOffset(2026, 3, 10, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2026, 3, 11, 0, 0, 0, TimeSpan.Zero)));
        Assert.Empty(e.OccurrencesBetween(new DateTimeOffset(2026, 3, 11, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2026, 3, 12, 0, 0, 0, TimeSpan.Zero)));
    }
}
