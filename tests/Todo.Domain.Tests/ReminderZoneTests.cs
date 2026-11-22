using Todo.Domain.Reminders;
using Todo.Domain.Settings;

namespace Todo.Domain.Tests;

public sealed class ReminderZoneTests
{
    private static readonly TimeZoneInfo Warsaw = TimeZoneInfo.FindSystemTimeZoneById("Europe/Warsaw");

    [Fact]
    public void LocalToUtc_UsesWinterAndSummerOffsets()
    {
        Assert.Equal(new DateTimeOffset(2030, 1, 10, 6, 30, 0, TimeSpan.Zero), ReminderRules.LocalToUtc(new DateOnly(2030, 1, 10), ReminderRules.DigestTime, Warsaw));
        Assert.Equal(new DateTimeOffset(2030, 7, 10, 5, 30, 0, TimeSpan.Zero), ReminderRules.LocalToUtc(new DateOnly(2030, 7, 10), ReminderRules.DigestTime, Warsaw));
    }

    [Fact]
    public void LocalToUtc_MovesASkippedTimeForward()
    {
        // 2030-03-31 02:30 does not exist in Warsaw (02:00 jumps to 03:00).
        Assert.Equal(new DateTimeOffset(2030, 3, 31, 1, 30, 0, TimeSpan.Zero), ReminderRules.LocalToUtc(new DateOnly(2030, 3, 31), new TimeOnly(2, 30), Warsaw));
    }

    [Fact]
    public void LocalToUtc_TakesTheFirstOfARepeatedTime()
    {
        // 2030-10-27 02:30 happens twice; the first one is still summer time (UTC+2).
        Assert.Equal(new DateTimeOffset(2030, 10, 27, 0, 30, 0, TimeSpan.Zero), ReminderRules.LocalToUtc(new DateOnly(2030, 10, 27), new TimeOnly(2, 30), Warsaw));
    }

    [Fact]
    public void ResolveZone_FallsBackToDefaultThenUtc()
    {
        Assert.Equal("Europe/Warsaw", ReminderRules.ResolveZone(null, "Europe/Warsaw").Id);
        Assert.Equal("Europe/Warsaw", ReminderRules.ResolveZone("Nope/Nowhere", "Europe/Warsaw").Id);
        Assert.Equal(TimeZoneInfo.Utc, ReminderRules.ResolveZone(null, "Nope/Nowhere"));
    }

    [Fact]
    public void Settings_AcceptKnownZoneAndRejectUnknownOne()
    {
        var time = new FixedTime(A.Now);
        var s = new PlannerSettings(Guid.NewGuid(), Guid.NewGuid(), A.Now);

        Assert.True(s.Update(null, null, DayOfWeek.Monday, time, "Europe/London").IsSuccess);
        Assert.Equal("Europe/London", s.TimeZoneId);
        Assert.True(s.Update(null, null, DayOfWeek.Monday, time, "Nope/Nowhere").IsFailed);
        Assert.Equal("Europe/London", s.TimeZoneId);
        Assert.True(s.Update(null, null, DayOfWeek.Monday, time).IsSuccess);
        Assert.Equal("Europe/London", s.TimeZoneId);
    }
}
