using Todo.Domain.Settings;

namespace Todo.Domain.Tests;

public sealed class PlannerSettingsTests
{
    private readonly FixedTime _time = new(A.Now);

    [Fact]
    public void Update_StoresRemindersAndFirstDay()
    {
        var s = new PlannerSettings(Guid.NewGuid(), Guid.NewGuid(), A.Now);

        var result = s.Update([30, 30, 60], [15], DayOfWeek.Sunday, _time);

        Assert.True(result.IsSuccess);
        Assert.Equal([30, 60], s.DefaultEventReminders.Select(r => r.MinutesBefore).ToArray());
        Assert.Equal([15], s.DefaultTaskReminders.Select(r => r.MinutesBefore).ToArray());
        Assert.Equal(DayOfWeek.Sunday, s.FirstDayOfWeek);
    }

    [Fact]
    public void Update_RejectsInvalidReminderAndKeepsState()
    {
        var s = new PlannerSettings(Guid.NewGuid(), Guid.NewGuid(), A.Now);

        var result = s.Update([-1], null, DayOfWeek.Sunday, _time);

        Assert.True(result.IsFailed);
        Assert.Equal(DayOfWeek.Monday, s.FirstDayOfWeek);
    }
}
