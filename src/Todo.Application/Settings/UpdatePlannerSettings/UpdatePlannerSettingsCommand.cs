using HomeApps.BuildingBlocks.Application.Messaging;
using HomeApps.Plugins;

namespace Todo.Application.Settings.UpdatePlannerSettings;

/// <summary>Every member, a guest too, changes only their own preferences (ADR 3.3).</summary>
public sealed record UpdatePlannerSettingsCommand(
    Guid HouseholdId, IReadOnlyList<int>? EventReminders, IReadOnlyList<int>? TaskReminders, DayOfWeek FirstDayOfWeek, string? TimeZone)
    : ICommand<PlannerSettingsView>, IHouseholdRequest
{
    public string Permission => TodoPlugin.Read;
}
