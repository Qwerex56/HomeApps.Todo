using HomeApps.BuildingBlocks.Application.Messaging;
using HomeApps.Plugins;

namespace Todo.Application.Events.CreateCalendarEvent;

/// <summary>Adds an event, or with an <c>Rrule</c> a recurring series. Returns the new event (the first occurrence for a series).</summary>
public sealed record CreateCalendarEventCommand(Guid HouseholdId, CalendarEventInput Event)
    : ICommand<CalendarEventDetailView>, IHouseholdRequest
{
    public string Permission => TodoPlugin.Write;
}
