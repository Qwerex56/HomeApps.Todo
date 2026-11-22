using HomeApps.BuildingBlocks.Application.Messaging;
using HomeApps.Plugins;

namespace Todo.Application.Events.GetCalendarEvent;

/// <summary>One event, or the first occurrence of a series when <c>EventId</c> is a series id. Hidden events are not found.</summary>
public sealed record GetCalendarEventQuery(Guid HouseholdId, Guid EventId) : IQuery<CalendarEventDetailView>, IHouseholdRequest
{
    public string Permission => TodoPlugin.Read;
}
