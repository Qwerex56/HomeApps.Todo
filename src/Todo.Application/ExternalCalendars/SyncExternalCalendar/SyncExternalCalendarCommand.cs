using HomeApps.BuildingBlocks.Application.Messaging;
using HomeApps.Plugins;

namespace Todo.Application.ExternalCalendars.SyncExternalCalendar;

/// <summary>Refreshes one of the caller's own imports now.</summary>
public sealed record SyncExternalCalendarCommand(Guid HouseholdId, Guid Id) : ICommand<ExternalCalendarView>, IHouseholdRequest
{
    public string Permission => TodoPlugin.Write;
}
