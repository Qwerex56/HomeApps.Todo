using Microsoft.EntityFrameworkCore;
using Todo.Application.Abstractions;
using Todo.Domain.Households;

namespace Todo.Infrastructure.Persistence;

internal sealed class HouseholdMemberRepository(TodoDbContext db) : IHouseholdMemberRepository
{
    public Task<HouseholdMemberProjection?> GetAsync(Guid householdId, Guid userId, CancellationToken cancellationToken) =>
        db.HouseholdMembers.FirstOrDefaultAsync(m => m.HouseholdId == householdId && m.UserId == userId, cancellationToken);

    public async Task<IReadOnlyList<HouseholdMemberProjection>> ListOfUserAsync(Guid userId, CancellationToken cancellationToken) =>
        await db.HouseholdMembers.Where(m => m.UserId == userId).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<HouseholdMemberProjection>> ListOfHouseholdAsync(Guid householdId, CancellationToken cancellationToken) =>
        await db.HouseholdMembers.Where(m => m.HouseholdId == householdId).ToListAsync(cancellationToken);

    public void Add(HouseholdMemberProjection member) => db.HouseholdMembers.Add(member);
}
