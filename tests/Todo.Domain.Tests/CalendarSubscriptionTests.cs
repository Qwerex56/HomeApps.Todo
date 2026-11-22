using Todo.Domain.Events;

namespace Todo.Domain.Tests;

public sealed class CalendarSubscriptionTests
{
    private static CalendarSubscription New() =>
        new(Guid.NewGuid(), Guid.NewGuid(), "hash", " Phone ", true, false, SubscriptionTasksMode.Assigned, A.Now);

    [Fact]
    public void New_IsActiveAndServesWithTrimmedLabel()
    {
        var s = New();

        Assert.Equal(("Phone", true), (s.Label, s.Serves));
    }

    [Fact]
    public void Revoke_StopsServing()
    {
        var s = New();
        s.Revoke();

        Assert.False(s.Serves);
    }

    [Fact]
    public void SuspendAndResume_ToggleServing()
    {
        var s = New();
        s.Suspend();
        var suspended = s.Serves;
        s.Resume();

        Assert.Equal((false, true), (suspended, s.Serves));
    }
}
