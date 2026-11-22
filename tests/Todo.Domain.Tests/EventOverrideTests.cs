using Todo.Domain.Common;
using Todo.Domain.Events;

namespace Todo.Domain.Tests;

public sealed class EventOverrideTests
{
    private readonly FixedTime _time = new(A.Now);
    private readonly Guid _series = Guid.NewGuid();
    private static readonly DateOnly Instance = new(2026, 11, 13);

    [Fact]
    public void Cancel_MarksOnlyThatOccurrenceAsCancelled()
    {
        var o = EventOverride.Cancel(_series, Instance, _time);

        Assert.Equal((_series, Instance, true, (string?)null), (o.SeriesId, o.OriginalInstanceDate, o.IsCancelled, o.OverrideTitle?.Value));
    }

    [Fact]
    public void Change_CarriesTheNewTitleAndTimesOfTheOccurrence()
    {
        var start = A.Now.AddDays(7);
        var end = start.AddHours(2);

        var o = EventOverride.Change(_series, Instance, "Trening przeniesiony", start, end, [30], _time).Value;

        Assert.Equal(("Trening przeniesiony", start, end, false, 30),
            (o.OverrideTitle!.Value, o.OverrideStartUtc, o.OverrideEndUtc, o.IsCancelled, o.OverrideReminders!.Single().MinutesBefore));
    }

    [Fact]
    public void Change_RejectsEndBeforeStart()
    {
        var start = A.Now.AddDays(7);

        var result = EventOverride.Change(_series, Instance, null, start, start.AddHours(-1), null, _time);

        Assert.Equal("planner.invalid_date_range", result.Code());
    }

    [Fact]
    public void Change_RejectsEmptyTitleAndInvalidReminder()
    {
        Assert.Equal(("planner.empty_title", "planner.invalid_reminder"),
            (EventOverride.Change(_series, Instance, "  ", null, null, null, _time).Code(),
             EventOverride.Change(_series, Instance, null, null, null, [-5], _time).Code()));
    }

    [Fact]
    public void Replace_UnCancelsACancelledOccurrence()
    {
        var o = EventOverride.Cancel(_series, Instance, _time);

        var result = o.Replace("Wracam", null, null, null, _time);

        Assert.Equal((true, false), (result.IsSuccess, o.IsCancelled));
        Assert.Equal("Wracam", o.OverrideTitle!.Value);
    }

    [Fact]
    public void Replace_WithNullFieldsKeepsTheCurrentValues()
    {
        var start = A.Now.AddDays(7);
        var o = EventOverride.Change(_series, Instance, "Nowy tytuł", start, start.AddHours(1), null, _time).Value;

        o.Replace(null, null, null, [15], _time);

        Assert.Equal(("Nowy tytuł", (DateTimeOffset?)start, 15), (o.OverrideTitle?.Value, o.OverrideStartUtc, o.OverrideReminders!.Single().MinutesBefore));
    }

    [Fact]
    public void CancelOccurrence_ClearsEveryOverriddenField()
    {
        var start = A.Now.AddDays(7);
        var o = EventOverride.Change(_series, Instance, "Coś", start, start.AddHours(1), [10], _time).Value;

        o.CancelOccurrence(_time);

        Assert.Equal((true, (string?)null, (DateTimeOffset?)null, (DateTimeOffset?)null),
            (o.IsCancelled, o.OverrideTitle?.Value, o.OverrideStartUtc, o.OverrideEndUtc));
        Assert.Null(o.OverrideReminders);
    }
}
