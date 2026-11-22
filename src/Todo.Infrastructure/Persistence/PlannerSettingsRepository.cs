using Microsoft.EntityFrameworkCore;
using Todo.Application.Abstractions;
using Todo.Domain.Settings;

namespace Todo.Infrastructure.Persistence;

internal sealed class PlannerSettingsRepository(TodoDbContext db) : IPlannerSettingsRepository
{
    public Task<PlannerSettings?> GetAsync(Guid householdId, Guid userId, CancellationToken cancellationToken) =>
        db.PlannerSettings.FirstOrDefaultAsync(s => s.HouseholdId == householdId && s.UserId == userId, cancellationToken);

    public void Add(PlannerSettings settings) => db.PlannerSettings.Add(settings);
}
