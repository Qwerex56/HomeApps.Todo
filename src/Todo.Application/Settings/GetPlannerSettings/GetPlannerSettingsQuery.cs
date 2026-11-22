using HomeApps.BuildingBlocks.Application.Messaging;
using HomeApps.Plugins;

namespace Todo.Application.Settings.GetPlannerSettings;

/// <summary>The caller's own planner preferences in a household; defaults when never saved.</summary>
public sealed record GetPlannerSettingsQuery(Guid HouseholdId) : IQuery<PlannerSettingsView>, IHouseholdRequest
{
    public string Permission => TodoPlugin.Read;
}
