namespace Todo.Domain.Households;

/// <summary>
/// The smallest plugin-owned fact background work needs without a caller token, derived only from event payloads
/// (no roles, no names): <c>(household, null)</c> the household is archived, <c>(null, user)</c> the account is
/// disabled or erased, <c>(household, user)</c> the user left the household.
/// </summary>
public sealed class InactiveMarker
{
    private InactiveMarker() { }

    public InactiveMarker(Guid? householdId, Guid? userId, DateTimeOffset now)
    {
        if (householdId is null && userId is null)
        {
            throw new ArgumentException("A marker needs a household, a user or both.");
        }

        Id = Guid.CreateVersion7(now);
        HouseholdId = householdId;
        UserId = userId;
        CreatedAt = now;
    }

    public Guid Id { get; private set; }

    public Guid? HouseholdId { get; private set; }

    public Guid? UserId { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
}
