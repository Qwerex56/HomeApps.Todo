using Todo.Domain.Common;
using Todo.Domain.Events;

namespace Todo.Domain.Tests;

public sealed class EventSeriesOccurrenceTests
{
    private readonly FixedTime _time = new(A.Now);
    private readonly Guid _household = Guid.NewGuid();
    private readonly Actor _author = Actor.Member(Guid.NewGuid());

    // Tuesdays and Thursdays 16:00-17:00 Warsaw, first on Tuesday 2026-10-20 (CEST).
    private EventSeries Weekly(bool isPrivate = false) => EventSeries.Create(_household, _author, "Trening", null, null, "Europe/Warsaw",
        new DateTimeOffset(2026, 10, 20, 14, 0, 0, TimeSpan.Zero), new DateTimeOffset(2026, 10, 20, 15, 0, 0, TimeSpan.Zero),
        false, isPrivate, "FREQ=WEEKLY;BYDAY=TU,TH", null, _time).Value;

    [Fact]
    public void Occurrences_KeepLocalTimeAcrossTheDaylightSavingChange()
    {
        var series = Weekly();

        var found = series.OccurrencesBetween(new DateTimeOffset(2026, 10, 20, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2026, 11, 3, 0, 0, 0, TimeSpan.Zero), []);

        // 2026-10-25 the clocks go back: 16:00 is 14:00 UTC before, 15:00 UTC after.
        Assert.Equal(
            [new(2026, 10, 20, 14, 0, 0, TimeSpan.Zero), new(2026, 10, 22, 14, 0, 0, TimeSpan.Zero),
             new(2026, 10, 27, 15, 0, 0, TimeSpan.Zero), new(2026, 10, 29, 15, 0, 0, TimeSpan.Zero)],
            found.Select(o => o.StartUtc));
    }

    [Fact]
    public void Occurrences_LeaveOutCancelledAndApplyChanges()
    {
        var series = Weekly();
        var cancelled = EventOverride.Cancel(series.Id, new DateOnly(2026, 10, 22), _time);
        var renamed = EventOverride.Change(series.Id, new DateOnly(2026, 10, 27), "Mecz", null, null, [30], _time).Value;

        var found = series.OccurrencesBetween(new DateTimeOffset(2026, 10, 20, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2026, 10, 30, 0, 0, 0, TimeSpan.Zero), [cancelled, renamed]);

        Assert.Equal(["Trening", "Mecz", "Trening"], found.Select(o => o.Title));
        Assert.Equal([30], found[1].Reminders);
    }

    [Fact]
    public void Occurrences_FollowAMovedOccurrenceIntoTheWindow()
    {
        var series = Weekly();
        var moved = EventOverride.Change(series.Id, new DateOnly(2026, 10, 22), null, new DateTimeOffset(2026, 11, 20, 9, 0, 0, TimeSpan.Zero), null, null, _time).Value;

        var found = series.OccurrencesBetween(new DateTimeOffset(2026, 11, 19, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2026, 11, 21, 0, 0, 0, TimeSpan.Zero), [moved]);

        Assert.Contains(found, o => o.StartUtc == new DateTimeOffset(2026, 11, 20, 9, 0, 0, TimeSpan.Zero) && o.EndUtc - o.StartUtc == TimeSpan.FromHours(1));
    }

    [Fact]
    public void EndAt_StopsLaterOccurrences()
    {
        var series = Weekly();
        series.EndAt(new DateTimeOffset(2026, 10, 26, 0, 0, 0, TimeSpan.Zero), _time);

        var found = series.OccurrencesBetween(new DateTimeOffset(2026, 10, 1, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2026, 12, 1, 0, 0, 0, TimeSpan.Zero), []);

        Assert.Equal(2, found.Count);
    }

    [Fact]
    public void OccurrenceOn_IsNullOffTheRuleAndWhenCancelled()
    {
        var series = Weekly();

        Assert.Null(series.OccurrenceOn(new DateOnly(2026, 10, 21), null));
        Assert.Null(series.OccurrenceOn(new DateOnly(2026, 10, 22), EventOverride.Cancel(series.Id, new DateOnly(2026, 10, 22), _time)));
        Assert.NotNull(series.OccurrenceOn(new DateOnly(2026, 10, 22), null));
    }

    [Fact]
    public void Edit_ChangesAllOccurrencesButKeepsTheFirstDay()
    {
        var series = Weekly();

        var result = series.Edit(_author, "Basen", null, null, new TimeOnly(18, 0), new TimeOnly(19, 0), false, "FREQ=WEEKLY;BYDAY=TU", _time);

        Assert.True(result.IsSuccess);
        Assert.Equal(new DateOnly(2026, 10, 20), series.StartDate);
        Assert.Equal(new DateTimeOffset(2026, 10, 20, 16, 0, 0, TimeSpan.Zero), series.FirstOccurrence()!.StartUtc);
        Assert.Equal("Basen", series.FirstOccurrence()!.Title);
    }

    [Fact]
    public void Create_RejectsAnOccurrenceLongerThanADay()
    {
        var result = EventSeries.Create(_household, _author, "x", null, null, "Europe/Warsaw", A.Now, A.Now.AddDays(2), false, false,
            "FREQ=DAILY", null, _time);

        Assert.Equal("planner.invalid_date_range", result.Code());
    }

    [Fact]
    public void PrivateSeries_IsHiddenFromOthersAndDeletedByItsAuthorOnly()
    {
        var series = Weekly(isPrivate: true);
        var other = Actor.Manager(Guid.NewGuid());

        Assert.Equal("planner.event_not_found", series.CanEdit(other).Code());
        Assert.True(series.CanDelete(_author).IsSuccess);
        Assert.False(series.IsVisibleToGuest);
    }

    [Fact]
    public void HouseholdSeries_IsDeletedByItsAuthorOrAnAdminOnly()
    {
        var series = Weekly();

        Assert.True(series.CanEdit(Actor.Member(Guid.NewGuid())).IsSuccess);
        Assert.Equal("planner.cannot_delete_event", series.CanDelete(Actor.Member(Guid.NewGuid())).Code());
        Assert.True(series.CanDelete(Actor.Manager(Guid.NewGuid())).IsSuccess);
    }
}
