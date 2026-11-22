using Todo.Domain.Events;

namespace Todo.Domain.Tests;

/// <summary>RFC 5545 expectations for RRULE combinations the evaluator must support (ADR 3.7).</summary>
public sealed class RecurrenceRfcTests
{
    private static DateOnly D(int y, int m, int d) => new(y, m, d);

    [Fact]
    public void Weekly_WithInterval_SkipsTheWeeksInBetween()
    {
        var dates = RecurrenceEvaluator.Dates("FREQ=WEEKLY;INTERVAL=2;BYDAY=MO", D(2026, 1, 5), null, D(2026, 1, 1), D(2026, 2, 1));

        Assert.Equal([D(2026, 1, 5), D(2026, 1, 19)], dates);
    }

    [Fact]
    public void Weekly_CountStartingMidWeek_CountsDaysBeforeTheStartAsNothing()
    {
        // Start is Thursday 2026-11-05: the Tuesday of that week is before the start and is not an occurrence.
        var dates = RecurrenceEvaluator.Dates("FREQ=WEEKLY;BYDAY=TU,TH;COUNT=3", D(2026, 11, 5), null, D(2026, 11, 1), D(2026, 12, 31));

        Assert.Equal([D(2026, 11, 5), D(2026, 11, 10), D(2026, 11, 12)], dates);
    }

    [Fact]
    public void Monthly_LastDayOfMonth_UsesNegativeMonthDay()
    {
        var dates = RecurrenceEvaluator.Dates("FREQ=MONTHLY;BYMONTHDAY=-1", D(2026, 1, 15), null, D(2026, 1, 1), D(2026, 3, 31));

        Assert.Equal([D(2026, 1, 31), D(2026, 2, 28), D(2026, 3, 31)], dates);
    }

    [Fact]
    public void Monthly_ByMonthDay31_SkipsMonthsWithoutA31st()
    {
        var dates = RecurrenceEvaluator.Dates("FREQ=MONTHLY;BYMONTHDAY=31", D(2026, 1, 1), null, D(2026, 1, 1), D(2026, 4, 30));

        Assert.Equal([D(2026, 1, 31), D(2026, 3, 31)], dates);
    }

    [Fact]
    public void Yearly_ByDayWithoutByMonth_IsEveryMatchingDayOfTheYear()
    {
        // RFC 5545: YEARLY;BYDAY=MO without BYMONTH means every Monday of the year, not only January's.
        var dates = RecurrenceEvaluator.Dates("FREQ=YEARLY;BYDAY=MO", D(2026, 1, 5), null, D(2026, 1, 1), D(2026, 2, 28));

        Assert.Equal([D(2026, 1, 5), D(2026, 1, 12), D(2026, 1, 19), D(2026, 1, 26), D(2026, 2, 2), D(2026, 2, 9), D(2026, 2, 16), D(2026, 2, 23)], dates);
    }

    [Fact]
    public void Yearly_SecondTuesdayOfAMonth_RepeatsEveryYear()
    {
        // 2026-03-10 is the second Tuesday of March; 2027-03-09 is the second Tuesday of March 2027.
        var dates = RecurrenceEvaluator.Dates("FREQ=YEARLY;BYMONTH=3;BYDAY=2TU", D(2026, 3, 10), null, D(2026, 1, 1), D(2027, 12, 31));

        Assert.Equal([D(2026, 3, 10), D(2027, 3, 9)], dates);
    }

    [Theory]
    [InlineData("FREQ=DAILY;BYMONTHDAY=1")]
    [InlineData("FREQ=WEEKLY;BYMONTHDAY=1")]
    public void IsSupported_RejectsMonthDayOnDailyAndWeekly_WhichItWouldIgnore(string rule)
    {
        // RFC 5545 allows BYMONTHDAY only with MONTHLY and YEARLY; the evaluator must not silently drop it.
        Assert.False(RecurrenceEvaluator.IsSupported(rule));
    }
}
