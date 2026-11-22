using HomeApps.BuildingBlocks.Application.Messaging;
using HomeApps.Plugins;

namespace Todo.Application.ExternalCalendars.RemoveExternalCalendar;

/// <summary>Deletes an import and with it its cached events; only its owner may.</summary>
public sealed record RemoveExternalCalendarCommand(Guid HouseholdId, Guid Id) : ICommand, IHouseholdRequest
{
    public string Permission => TodoPlugin.Write;
}
