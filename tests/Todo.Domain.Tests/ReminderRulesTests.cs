using Todo.Domain.Reminders;

namespace Todo.Domain.Tests;

public sealed class ReminderRulesTests
{
    private static readonly DateTimeOffset Now = A.Now;

    [Theory]
    [InlineData(-30, ReminderVerdict.Send)]
    [InlineData(0, ReminderVerdict.Send)]
    [InlineData(1, ReminderVerdict.Skip)]
    [InlineData(-60, ReminderVerdict.Send)]
    public void Judge_FiresOnlyInTheWindow(int secondsOrMinutes, ReminderVerdict expected)
    {
        var at = Math.Abs(secondsOrMinutes) == 60 ? Now.AddMinutes(-60) : Now.AddSeconds(secondsOrMinutes);
        Assert.Equal(expected, ReminderRules.Judge(at, Now.AddMinutes(-61), Now));
    }

    [Fact]
    public void Judge_DropsOverAnHourLate()
    {
        Assert.Equal(ReminderVerdict.Drop, ReminderRules.Judge(Now.AddMinutes(-61), Now.AddMinutes(-90), Now));
    }

    [Fact]
    public void Judge_SkipsWhatAnEarlierScanHandled()
    {
        Assert.Equal(ReminderVerdict.Skip, ReminderRules.Judge(Now.AddSeconds(-30), Now.AddSeconds(-30), Now));
    }

    [Fact]
    public void MinutesFor_OverrideReplacesEverythingEvenWhenEmpty()
    {
        Assert.Empty(ReminderRules.MinutesFor([10], [], [60]));
        Assert.Equal([5], ReminderRules.MinutesFor([10], [5], [60]));
    }

    [Fact]
    public void MinutesFor_EventRemindersBeatDefaults()
    {
        Assert.Equal([10], ReminderRules.MinutesFor([10], null, [60]));
        Assert.Equal([60], ReminderRules.MinutesFor([], null, [60]));
    }
}
