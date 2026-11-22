using FluentResults;
using Todo.Application.Abstractions;

namespace Todo.Api.Tests;

/// <summary>Answers each download with the next queued result and remembers the URLs asked for.</summary>
public sealed class FakeSsrfSafeClient : ISsrfSafeClient
{
    private readonly Queue<Result<string>> _answers = new();

    public List<string> Requested { get; } = [];

    public FakeSsrfSafeClient Answer(Result<string> result)
    {
        _answers.Enqueue(result);
        return this;
    }

    public Task<Result<string>> DownloadAsync(string url, CancellationToken cancellationToken)
    {
        Requested.Add(url);
        return Task.FromResult(_answers.Dequeue());
    }
}
