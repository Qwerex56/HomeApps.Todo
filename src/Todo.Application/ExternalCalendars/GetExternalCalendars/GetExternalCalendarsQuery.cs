using HomeApps.BuildingBlocks.Application.Messaging;
using HomeApps.Plugins;

namespace Todo.Application.ExternalCalendars.GetExternalCalendars;

/// <summary>The caller's own imports in a household.</summary>
public sealed record GetExternalCalendarsQuery(Guid HouseholdId) : IQuery<IReadOnlyList<ExternalCalendarView>>, IHouseholdRequest
{
    public string Permission => TodoPlugin.Write;
}
