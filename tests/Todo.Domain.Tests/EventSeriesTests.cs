using FluentResults;
using Todo.Domain.Common;
using Todo.Domain.Events;

namespace Todo.Domain.Tests;

public sealed class EventSeriesTests
{
    private readonly FixedTime _time = new(A.Now);
    private readonly Guid _household = Guid.NewGuid();
    private readonly Actor _author = Actor.Member(Guid.NewGuid());

    private Result<EventSeries> Create(string? rrule = "FREQ=WEEKLY;BYDAY=TU,TH", string? zone = "Europe/Warsaw",
        DateTimeOffset? end = null, string? title = "Trening") =>
        EventSeries.Create(_household, _author, title, null, null, zone, new TimeOnly(16, 0), new TimeOnly(17, 0), false, false,
            rrule, A.Now, end, _time);

    [Fact]
    public void Create_KeepsLocalTimeAndZoneTheSeriesRunsIn()
    {
        var s = Create().Value;

        Assert.Equal(("Europe/Warsaw", new TimeOnly(16, 0), new TimeOnly(17, 0), "FREQ=WEEKLY;BYDAY=TU,TH"),
            (s.TimeZoneId, s.LocalStartTime, s.LocalEndTime, s.Rule.Value));
    }

    [Fact]
    public void Create_RejectsSeriesEndingBeforeItStarts()
    {
        Assert.Equal("planner.invalid_date_range", Create(end: A.Now.AddDays(-1)).Code());
    }

    [Theory]
    [InlineData("BYDAY=TU")]
    [InlineData("FREQ=DAILY;X=<")]
    public void Create_RejectsMalformedRule(string rrule) => Assert.Equal("planner.invalid_rule", Create(rrule: rrule).Code());

    [Fact]
    public void Create_RejectsUnknownTimeZone() => Assert.Equal("planner.invalid_time_zone", Create(zone: "Mars/Olympus").Code());

    [Fact]
    public void Create_RejectsEmptyTitle() => Assert.Equal("planner.empty_title", Create(title: "   ").Code());

    [Fact]
    public void EndAt_SplitsTheSeriesAndRefusesAnEndBeforeTheStart()
    {
        var s = Create().Value;
        var until = A.Now.AddDays(30);

        var ok = s.EndAt(until, _time);
        var tooEarly = s.EndAt(A.Now.AddDays(-1), _time);

        Assert.Equal((true, until), (ok.IsSuccess, s.SeriesEndUtc));
        Assert.Equal("planner.invalid_date_range", tooEarly.Code());
        Assert.Equal(until, s.SeriesEndUtc);
    }

    [Fact]
    public void OwnerLeft_MarksOnlyAPrivateSeriesOfThatOwner()
    {
        var privateSeries = EventSeries.Create(_household, _author, "Prywatne", null, null, "Europe/Warsaw",
            new TimeOnly(9, 0), new TimeOnly(10, 0), false, true, "FREQ=DAILY", A.Now, null, _time).Value;
        var sharedSeries = Create().Value;

        privateSeries.OwnerLeft(Guid.NewGuid(), _time);
        sharedSeries.OwnerLeft(_author.UserId, _time);
        privateSeries.OwnerLeft(_author.UserId, _time);

        Assert.Null(sharedSeries.OwnerLeftAt);
        Assert.Equal(A.Now, privateSeries.OwnerLeftAt);
    }

    [Fact]
    public void OwnerReturned_ClearsTheMarkAndIsIdempotent()
    {
        var s = EventSeries.Create(_household, _author, "Prywatne", null, null, "Europe/Warsaw",
            new TimeOnly(9, 0), new TimeOnly(10, 0), false, true, "FREQ=DAILY", A.Now, null, _time).Value;
        s.OwnerLeft(_author.UserId, _time);

        s.OwnerReturned(_time);
        s.OwnerReturned(_time);

        Assert.Null(s.OwnerLeftAt);
    }
}
