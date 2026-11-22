using HomeApps.BuildingBlocks.Application.Messaging;
using HomeApps.Plugins;

namespace Todo.Application.Events.DeleteCalendarEvent;

/// <summary>A hard delete of a single event: by its author or a household admin; a private event by its author only.</summary>
public sealed record DeleteCalendarEventCommand(Guid HouseholdId, Guid EventId) : ICommand, IHouseholdRequest
{
    public string Permission => TodoPlugin.Write;
}
