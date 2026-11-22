using FluentResults;
using Todo.Application.ExternalCalendars;
using Todo.Domain.Common;
using Todo.Domain.Events;

namespace Todo.Api.Tests;

/// <summary>No Docker needed: the download is a fake, so these check only how the syncer records outcomes.</summary>
public sealed class ExternalCalendarSyncerTests
{
    private const string ValidIcs = "BEGIN:VCALENDAR\r\nBEGIN:VEVENT\r\nUID:a1\r\nSUMMARY:Dentist\r\nDTSTART:20260310T090000Z\r\nDTEND:20260310T100000Z\r\nEND:VEVENT\r\nEND:VCALENDAR\r\n";

    private static readonly DateTimeOffset Start = new(2026, 3, 1, 8, 0, 0, TimeSpan.Zero);

    private static ExternalCalendarImport NewImport(MovableTime time) =>
        ExternalCalendarImport.Create(Guid.NewGuid(), Guid.NewGuid(), "https://example.com/cal.ics", "Work", null, ExternalCalendarVisibility.Details, time).Value;

    [Fact]
    public async Task A_successful_download_is_cached_and_clears_the_failure_streak()
    {
        var time = new MovableTime();
        var import = NewImport(time);
        var client = new FakeSsrfSafeClient().Answer(Result.Fail<string>(PlannerErrors.SyncFailed)).Answer(Result.Ok(ValidIcs));
        var syncer = new ExternalCalendarSyncer(client, time);

        await syncer.SyncAsync(import, CancellationToken.None);
        var second = await syncer.SyncAsync(import, CancellationToken.None);

        Assert.True(second.IsSuccess);
        Assert.Equal((ValidIcs, 0, false), (import.CachedIcsData, import.SyncFailureCount, import.IsStale));
        Assert.NotNull(import.LastSyncedAt);
        Assert.Null(import.LastSyncFailedAt);
    }

    [Fact]
    public async Task A_failed_download_is_counted_and_keeps_the_last_good_data()
    {
        var time = new MovableTime();
        var import = NewImport(time);
        var client = new FakeSsrfSafeClient().Answer(Result.Ok(ValidIcs)).Answer(Result.Fail<string>(PlannerErrors.SyncFailed));
        var syncer = new ExternalCalendarSyncer(client, time);
        await syncer.SyncAsync(import, CancellationToken.None);

        time.Advance(TimeSpan.FromMinutes(30));
        var failed = await syncer.SyncAsync(import, CancellationToken.None);

        Assert.True(failed.IsFailed);
        Assert.Equal((PlannerErrors.SyncFailed.Message, 1, true), (import.LastErrorMessage, import.SyncFailureCount, import.IsStale));
        Assert.Equal(ValidIcs, import.CachedIcsData);
    }

    [Fact]
    public async Task A_refused_address_is_recorded_and_the_refusal_is_returned()
    {
        var time = new MovableTime();
        var import = NewImport(time);
        var syncer = new ExternalCalendarSyncer(new FakeSsrfSafeClient().Answer(Result.Fail<string>(PlannerErrors.SsrfRejected)), time);

        var result = await syncer.SyncAsync(import, CancellationToken.None);

        Assert.Equal("planner.ssrf_rejected", result.Errors[0].Metadata["Code"].ToString());
        Assert.Equal((1, null), (import.SyncFailureCount, import.CachedIcsData));
    }

    [Fact]
    public async Task A_download_that_is_not_a_calendar_counts_as_a_failure_and_caches_nothing()
    {
        var time = new MovableTime();
        var import = NewImport(time);
        var syncer = new ExternalCalendarSyncer(new FakeSsrfSafeClient().Answer(Result.Ok("<html>not a calendar</html>")), time);

        var result = await syncer.SyncAsync(import, CancellationToken.None);

        Assert.Equal("planner.parse_error", result.Errors[0].Metadata["Code"].ToString());
        Assert.Equal((1, null), (import.SyncFailureCount, import.CachedIcsData));
    }

    [Fact]
    public async Task Failures_count_up_to_the_three_day_mark_and_the_owner_is_told_once()
    {
        var time = new MovableTime();
        var import = NewImport(time);
        var client = new FakeSsrfSafeClient();
        for (var i = 0; i < ExternalCalendarImport.NotifyAfterFailures; i++)
        {
            client.Answer(Result.Fail<string>(PlannerErrors.SyncFailed));
        }

        var syncer = new ExternalCalendarSyncer(client, time);
        for (var i = 0; i < ExternalCalendarImport.NotifyAfterFailures; i++)
        {
            await syncer.SyncAsync(import, CancellationToken.None);
        }

        Assert.Equal((ExternalCalendarImport.NotifyAfterFailures, true), (import.SyncFailureCount, import.ShouldNotifyOwner));

        client.Answer(Result.Fail<string>(PlannerErrors.SyncFailed));
        await syncer.SyncAsync(import, CancellationToken.None);
        Assert.False(import.ShouldNotifyOwner);
    }

    [Fact]
    public async Task The_syncer_downloads_the_address_the_import_was_created_with()
    {
        var time = new MovableTime();
        var import = NewImport(time);
        var client = new FakeSsrfSafeClient().Answer(Result.Ok(ValidIcs));

        await new ExternalCalendarSyncer(client, time).SyncAsync(import, CancellationToken.None);

        Assert.Equal([import.Url], client.Requested);
    }
}
