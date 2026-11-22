using FluentResults;

namespace Todo.Application.Abstractions;

/// <summary>
/// Downloads an external calendar. Refuses addresses inside the server's network (ADR 3.6): a failed result is
/// <c>planner.ssrf_rejected</c> for such an address, or <c>planner.sync_failed</c> for any other download problem.
/// </summary>
public interface ISsrfSafeClient
{
    Task<Result<string>> DownloadAsync(string url, CancellationToken cancellationToken);
}
