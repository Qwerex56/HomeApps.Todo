namespace Todo.Api.Tests;

/// <summary>A clock the tests can move; starts at the real time so test tokens stay valid.</summary>
public sealed class MovableTime : TimeProvider
{
    private TimeSpan _offset;

    public override DateTimeOffset GetUtcNow() => System.GetUtcNow() + _offset;

    public void Advance(TimeSpan by) => _offset += by;

    public void Reset() => _offset = TimeSpan.Zero;
}
