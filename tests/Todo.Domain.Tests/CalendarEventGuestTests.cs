using Todo.Domain.Common;
using Todo.Domain.Events;

namespace Todo.Domain.Tests;

public sealed class CalendarEventGuestTests
{
    private readonly FixedTime _time = new(A.Now);
    private readonly Actor _author = Actor.Member(Guid.NewGuid());

    private CalendarEvent Make(bool isPrivate = false, params Guid[] attendees) => CalendarEvent.Create(Guid.NewGuid(), _author, "Obiad", null, null,
        "Europe/Warsaw", A.Now, A.Now.AddHours(1), false, isPrivate, attendees, null, _time).Value;

    [Fact]
    public void Guest_SeesWholeHouseholdEvents()
    {
        Assert.True(Make().IsVisibleToGuest(Guid.NewGuid()));
    }

    [Fact]
    public void Guest_SeesEventsOnlyWhenAttending()
    {
        var guest = Guid.NewGuid();
        var e = Make(false, Guid.NewGuid());

        Assert.False(e.IsVisibleToGuest(guest));
        Assert.True(Make(false, guest).IsVisibleToGuest(guest));
    }

    [Fact]
    public void Guest_NeverSeesPrivateEvents()
    {
        Assert.False(Make(true).IsVisibleToGuest(_author.UserId));
    }

    [Fact]
    public void Member_NotAttendingOrAuthor_CannotEditAnEventForSomeoneElse()
    {
        var e = Make(false, Guid.NewGuid());

        Assert.Equal("planner.cannot_edit_event", e.CanEdit(Actor.Member(Guid.NewGuid())).Code());
    }
}
