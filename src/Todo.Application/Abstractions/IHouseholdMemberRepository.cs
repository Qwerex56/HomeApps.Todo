using Todo.Domain.Households;

namespace Todo.Application.Abstractions;

/// <summary>Loads and stores the local <see cref="HouseholdMemberProjection"/>. Saving is the unit of work's job.</summary>
public interface IHouseholdMemberRepository
{
    Task<HouseholdMemberProjection?> GetAsync(Guid householdId, Guid userId, CancellationToken cancellationToken);

    Task<IReadOnlyList<HouseholdMemberProjection>> ListOfUserAsync(Guid userId, CancellationToken cancellationToken);

    Task<IReadOnlyList<HouseholdMemberProjection>> ListOfHouseholdAsync(Guid householdId, CancellationToken cancellationToken);

    void Add(HouseholdMemberProjection member);
}
