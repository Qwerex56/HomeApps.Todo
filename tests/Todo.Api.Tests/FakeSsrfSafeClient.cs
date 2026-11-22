using FluentResults;
using Todo.Application.Abstractions;

namespace Todo.Api.Tests;

/// <summary>Answers each download with the next queued result and remembers the URLs asked for.</summary>
public sealed class FakeSsrfSafeClient : ISsrfSafeClient
{
    private readonly Queue<Result<string>> _answers = new();

    /// <summary>Answers when nothing is queued (the factory passes the real client, so refusal tests keep working).</summary>
    public ISsrfSafeClient? Fallback { get; set; }

    public List<string> Requested { get; } = [];

    public FakeSsrfSafeClient Answer(Result<string> result)
    {
        _answers.Enqueue(result);
        return this;
    }

    public Task<Result<string>> DownloadAsync(string url, CancellationToken cancellationToken)
    {
        Requested.Add(url);
        return _answers.Count == 0 && Fallback is not null
            ? Fallback.DownloadAsync(url, cancellationToken)
            : Task.FromResult(_answers.Dequeue());
    }
}
