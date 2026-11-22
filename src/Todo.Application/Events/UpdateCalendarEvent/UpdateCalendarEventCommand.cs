using HomeApps.BuildingBlocks.Application.Messaging;
using HomeApps.Plugins;

namespace Todo.Application.Events.UpdateCalendarEvent;

/// <summary>Replaces a single event. Edits to a series go through the instance route; <c>Rrule</c> is ignored here.</summary>
public sealed record UpdateCalendarEventCommand(Guid HouseholdId, Guid EventId, CalendarEventInput Event)
    : ICommand<CalendarEventDetailView>, IHouseholdRequest
{
    public string Permission => TodoPlugin.Write;
}
