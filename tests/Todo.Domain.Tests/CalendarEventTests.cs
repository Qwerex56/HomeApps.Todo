using Todo.Domain.Common;
using Todo.Domain.Events;

namespace Todo.Domain.Tests;

public sealed class CalendarEventTests
{
    private readonly FixedTime _time = new(A.Now);
    private readonly Guid _household = Guid.NewGuid();

    private CalendarEvent New(Actor by, bool isPrivate = false, Guid[]? attendees = null) =>
        CalendarEvent.Create(_household, by, " Obiad ", null, null, "Europe/Warsaw", A.Now, A.Now.AddHours(1), false, isPrivate,
            attendees, [10], _time).Value;

    [Fact]
    public void Create_KeepsLocalTimeInTheZone()
    {
        var e = New(Actor.Member(Guid.NewGuid()));

        Assert.Equal(("Obiad", new TimeOnly(13, 0), new TimeOnly(14, 0)), (e.Title.Value, e.LocalStartTime, e.LocalEndTime));
        Assert.Equal(10, e.Reminders.Single().MinutesBefore);
    }

    [Fact]
    public void Create_RejectsEndBeforeStart()
    {
        var result = CalendarEvent.Create(_household, Actor.Member(Guid.NewGuid()), "x", null, null, "Europe/Warsaw",
            A.Now, A.Now.AddHours(-1), false, false, null, null, _time);

        Assert.Equal("planner.invalid_date_range", result.Code());
    }

    [Fact]
    public void Private_IsHiddenFromAdmins()
    {
        var author = Actor.Member(Guid.NewGuid());
        var e = New(author, isPrivate: true);

        Assert.True(e.CanSee(author));
        Assert.False(e.CanSee(Actor.Manager(Guid.NewGuid())));
        Assert.Equal("planner.event_not_found", e.CanDelete(Actor.Manager(Guid.NewGuid())).Code());
    }

    [Fact]
    public void MemberLeft_DropsAttendeeAndHidesTheirPrivateEvent()
    {
        var author = Guid.NewGuid();
        var shared = New(Actor.Member(Guid.NewGuid()), attendees: [author, Guid.NewGuid()]);
        var own = New(Actor.Member(author), isPrivate: true);

        shared.MemberLeft(author, _time);
        own.MemberLeft(author, _time);

        Assert.DoesNotContain(author, shared.Attendees);
        Assert.False(own.CanSee(Actor.Member(author)));
    }

    [Fact]
    public void Edit_ByNonAttendeeIsForbidden()
    {
        var e = New(Actor.Member(Guid.NewGuid()), attendees: [Guid.NewGuid()]);

        Assert.Equal("planner.cannot_edit_event", e.CanEdit(Actor.Member(Guid.NewGuid())).Code());
    }

    [Theory]
    [InlineData("FREQ=WEEKLY;BYDAY=TU,TH", true)]
    [InlineData("rrule:freq=daily", true)]
    [InlineData("BYDAY=TU", false)]
    [InlineData("FREQ=DAILY;X=<", false)]
    public void RecurrenceRule_ChecksShape(string raw, bool ok) => Assert.Equal(ok, RecurrenceRule.Create(raw).IsSuccess);
}
