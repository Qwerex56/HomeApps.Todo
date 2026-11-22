using Todo.Domain.Settings;

namespace Todo.Application.Abstractions;

/// <summary>Loads and stores <see cref="PlannerSettings"/>. Saving is the unit of work's job.</summary>
public interface IPlannerSettingsRepository
{
    Task<PlannerSettings?> GetAsync(Guid householdId, Guid userId, CancellationToken cancellationToken);

    void Add(PlannerSettings settings);
}
