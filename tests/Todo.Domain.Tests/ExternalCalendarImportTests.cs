using Todo.Domain.Events;

namespace Todo.Domain.Tests;

public sealed class ExternalCalendarImportTests
{
    private readonly FixedTime _time = new(A.Now);

    private ExternalCalendarImport Valid() =>
        ExternalCalendarImport.Create(Guid.NewGuid(), Guid.NewGuid(), "https://example.com/a.ics", "Szkola", null, ExternalCalendarVisibility.Busy, _time).Value;

    [Fact]
    public void Create_DefaultsTheColor()
    {
        Assert.Equal("#3b82f6", Valid().Color);
    }

    [Theory]
    [InlineData("not a url", "Name", null)]
    [InlineData("https://example.com/a.ics", " ", null)]
    [InlineData("https://example.com/a.ics", "Name", "12345678901234567")]
    public void Create_RejectsInvalidInput(string url, string name, string? color)
    {
        var result = ExternalCalendarImport.Create(Guid.NewGuid(), Guid.NewGuid(), url, name, color, ExternalCalendarVisibility.Busy, _time);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public void Synced_ClearsFailures()
    {
        var i = Valid();
        i.SyncFailed("boom", A.Now);

        i.Synced("ICS", A.Now.AddHours(1));

        Assert.Equal(("ICS", 0, (string?)null), (i.CachedIcsData, i.SyncFailureCount, i.LastErrorMessage));
    }

    [Fact]
    public void SyncFailed_CountsAndTruncatesTheMessage()
    {
        var i = Valid();

        i.SyncFailed(new string('x', 600), A.Now);
        i.SyncFailed("again", A.Now);

        Assert.Equal((2, "again"), (i.SyncFailureCount, i.LastErrorMessage));
    }

    [Fact]
    public void OwnerLeftAndReturned_TogglePause()
    {
        var i = Valid();

        i.OwnerLeft(A.Now);
        var paused = i.IsPaused;
        i.OwnerReturned();

        Assert.Equal((true, false), (paused, i.IsPaused));
    }

    [Fact]
    public void IsDueForSync_WhenNeverTriedOrLastTryIsHalfAnHourOld()
    {
        var c = Valid();
        Assert.True(c.IsDueForSync(A.Now));

        c.Synced("x", A.Now);
        Assert.False(c.IsDueForSync(A.Now.AddMinutes(29)));
        Assert.True(c.IsDueForSync(A.Now.AddMinutes(30)));

        c.SyncFailed("boom", A.Now.AddMinutes(30));
        Assert.False(c.IsDueForSync(A.Now.AddMinutes(45)));
        Assert.True(c.IsDueForSync(A.Now.AddMinutes(60)));
    }

    [Fact]
    public void IsDueForSync_SkipsOwnerWhoLeftUntilHeReturns()
    {
        var c = Valid();
        c.OwnerLeft(A.Now);

        Assert.False(c.IsDueForSync(A.Now.AddDays(1)));
        c.OwnerReturned();
        Assert.True(c.IsDueForSync(A.Now.AddDays(1)));
    }

    [Fact]
    public void IsDueForPurge_After30DaysOfAbsence()
    {
        var c = Valid();
        Assert.False(c.IsDueForPurge(A.Now.AddDays(90)));

        c.OwnerLeft(A.Now);
        Assert.False(c.IsDueForPurge(A.Now.AddDays(30).AddSeconds(-1)));
        Assert.True(c.IsDueForPurge(A.Now.AddDays(30)));
    }

    [Fact]
    public void ShouldNotifyOwner_OnlyOnTheThreeDayMark()
    {
        var c = Valid();
        for (var i = 1; i <= ExternalCalendarImport.NotifyAfterFailures + 2; i++)
        {
            c.SyncFailed("boom", A.Now);
            Assert.Equal(i == ExternalCalendarImport.NotifyAfterFailures, c.ShouldNotifyOwner);
        }
    }
}
