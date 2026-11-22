using FluentResults;
using Todo.Application.Abstractions;
using Todo.Domain.Events;

namespace Todo.Application.ExternalCalendars;

/// <summary>One synchronisation of an import: download, check it parses, store it. A failure is recorded on the import.</summary>
public sealed class ExternalCalendarSyncer(ISsrfSafeClient client, TimeProvider time)
{
    public async Task<Result> SyncAsync(ExternalCalendarImport import, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(import);
        var downloaded = await client.DownloadAsync(import.Url, cancellationToken);
        var failure = downloaded.IsFailed ? downloaded.ToResult() : IcsParser.Parse(downloaded.Value).ToResult();
        if (failure.IsFailed)
        {
            import.SyncFailed(failure.Errors[0].Message, time.GetUtcNow());
            return failure;
        }

        import.Synced(downloaded.Value, time.GetUtcNow());
        return Result.Ok();
    }
}
