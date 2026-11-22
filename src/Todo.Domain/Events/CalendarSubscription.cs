namespace Todo.Domain.Events;

/// <summary>
/// An ICS feed link for a phone calendar. Only the SHA-256 hash of the 256-bit token is kept (ADR decision 7);
/// the token itself is shown once, when it is made.
/// </summary>
public sealed class CalendarSubscription
{
    public const int MaxLabelLength = 100;

    private CalendarSubscription() { }

    public CalendarSubscription(
        Guid householdId, Guid userId, string tokenHash, string label, bool includeHouseholdEvents, bool includePrivateEvents,
        SubscriptionTasksMode tasksMode, DateTimeOffset now)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tokenHash);
        Id = Guid.CreateVersion7(now);
        HouseholdId = householdId;
        UserId = userId;
        TokenHash = tokenHash;
        Label = label.Trim().Length > MaxLabelLength ? label.Trim()[..MaxLabelLength] : label.Trim();
        IncludeHouseholdEvents = includeHouseholdEvents;
        IncludePrivateEvents = includePrivateEvents;
        TasksMode = tasksMode;
        CreatedAt = now;
        IsActive = true;
    }

    public Guid Id { get; private set; }

    public Guid HouseholdId { get; private set; }

    public Guid UserId { get; private set; }

    public string TokenHash { get; private set; } = null!;

    public string Label { get; private set; } = null!;

    public bool IncludeHouseholdEvents { get; private set; }

    public bool IncludePrivateEvents { get; private set; }

    public SubscriptionTasksMode TasksMode { get; private set; }

    public DateTimeOffset? LastAccessedAt { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public bool IsActive { get; private set; }

    /// <summary>True while the user's account is disabled; the feed answers 404 meanwhile.</summary>
    public bool IsSuspended { get; private set; }

    /// <summary>True while the owner is a guest in the household (from <c>GetMembershipAsync</c>, then <c>MemberRoleChanged</c>); guests get no private or attendee-only events.</summary>
    public bool IsGuest { get; private set; }

    public bool Serves => IsActive && !IsSuspended;

    public void Revoke() => IsActive = false;

    public void MarkGuest(bool guest) => IsGuest = guest;

    public void Suspend() => IsSuspended = true;

    public void Resume() => IsSuspended = false;

    public void Accessed(DateTimeOffset now) => LastAccessedAt = now;
}
