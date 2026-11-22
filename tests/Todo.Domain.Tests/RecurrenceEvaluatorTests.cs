using Todo.Domain.Events;

namespace Todo.Domain.Tests;

public sealed class RecurrenceEvaluatorTests
{
    private static DateOnly D(int y, int m, int d) => new(y, m, d);

    [Fact]
    public void Daily_WithInterval_StepsFromTheStart()
    {
        var dates = RecurrenceEvaluator.Dates("FREQ=DAILY;INTERVAL=2", D(2026, 1, 1), null, D(2026, 1, 1), D(2026, 1, 8));

        Assert.Equal([D(2026, 1, 1), D(2026, 1, 3), D(2026, 1, 5), D(2026, 1, 7)], dates);
    }

    [Fact]
    public void Weekly_ByDay_ListsTheDaysOfEachWeekInOrder()
    {
        var dates = RecurrenceEvaluator.Dates("FREQ=WEEKLY;BYDAY=TU,TH", D(2026, 11, 2), null, D(2026, 11, 2), D(2026, 11, 15));

        Assert.Equal([D(2026, 11, 3), D(2026, 11, 5), D(2026, 11, 10), D(2026, 11, 12)], dates);
    }

    [Fact]
    public void Count_CountsFromTheStartEvenWhenTheWindowIsLater()
    {
        var dates = RecurrenceEvaluator.Dates("FREQ=DAILY;COUNT=5", D(2026, 1, 1), null, D(2026, 1, 4), D(2026, 12, 31));

        Assert.Equal([D(2026, 1, 4), D(2026, 1, 5)], dates);
    }

    [Fact]
    public void Until_EndsTheSeriesInclusive()
    {
        var dates = RecurrenceEvaluator.Dates("FREQ=DAILY;UNTIL=20260103", D(2026, 1, 1), null, D(2026, 1, 1), D(2026, 12, 31));

        Assert.Equal(3, dates.Count);
    }

    [Fact]
    public void Monthly_LastFriday_UsesTheNegativeOrdinal()
    {
        var dates = RecurrenceEvaluator.Dates("FREQ=MONTHLY;BYDAY=-1FR", D(2026, 1, 1), null, D(2026, 1, 1), D(2026, 3, 31));

        Assert.Equal([D(2026, 1, 30), D(2026, 2, 27), D(2026, 3, 27)], dates);
    }

    [Fact]
    public void Monthly_DayThatDoesNotExistIsSkipped()
    {
        var dates = RecurrenceEvaluator.Dates("FREQ=MONTHLY", D(2026, 1, 31), null, D(2026, 1, 1), D(2026, 4, 30));

        Assert.Equal([D(2026, 1, 31), D(2026, 3, 31)], dates);
    }

    [Fact]
    public void Yearly_KeepsMonthAndDay()
    {
        var dates = RecurrenceEvaluator.Dates("FREQ=YEARLY", D(2024, 5, 17), null, D(2024, 1, 1), D(2026, 12, 31));

        Assert.Equal([D(2024, 5, 17), D(2025, 5, 17), D(2026, 5, 17)], dates);
    }

    [Fact]
    public void Dates_NeverReturnMoreThanTheCap()
    {
        var dates = RecurrenceEvaluator.Dates("FREQ=DAILY", D(2000, 1, 1), null, D(2000, 1, 1), D(2030, 1, 1));

        Assert.Equal(RecurrenceEvaluator.MaxOccurrences, dates.Count);
    }

    [Theory]
    [InlineData("FREQ=SECONDLY")]
    [InlineData("FREQ=DAILY;INTERVAL=0")]
    [InlineData("FREQ=WEEKLY;BYSETPOS=1")]
    [InlineData("FREQ=WEEKLY;BYDAY=1MO")]
    [InlineData("FREQ=DAILY;UNTIL=oops")]
    public void IsSupported_RejectsWhatItCannotEvaluate(string rule)
    {
        Assert.False(RecurrenceEvaluator.IsSupported(rule));
    }
}
