namespace Todo.Domain.Households;

/// <summary>Local copy of a member's role and account state, kept current by integration events (ADR decision 14).</summary>
public sealed class HouseholdMemberProjection
{
    private HouseholdMemberProjection() { }

    public HouseholdMemberProjection(Guid householdId, Guid userId, string role, DateTimeOffset joinedAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(role);
        Id = Guid.CreateVersion7(joinedAt);
        HouseholdId = householdId;
        UserId = userId;
        Role = role;
        JoinedAt = joinedAt;
    }

    public Guid Id { get; private set; }

    public Guid HouseholdId { get; private set; }

    public Guid UserId { get; private set; }

    public string Role { get; private set; } = null!;

    public bool IsDisabled { get; private set; }

    public DateTimeOffset JoinedAt { get; private set; }

    public DateTimeOffset? LeftAt { get; private set; }

    public bool IsActive => LeftAt is null && !IsDisabled;

    public void ChangeRole(string role)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(role);
        Role = role;
    }

    public void Left(DateTimeOffset now) => LeftAt ??= now;

    public void Returned(string role, DateTimeOffset now)
    {
        ChangeRole(role);
        LeftAt = null;
        JoinedAt = now;
    }

    public void Disable() => IsDisabled = true;

    public void Enable() => IsDisabled = false;
}
