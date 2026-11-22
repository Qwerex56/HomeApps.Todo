using HomeApps.BuildingBlocks.Application.Messaging;
using HomeApps.Plugins;
using Todo.Domain.Events;

namespace Todo.Application.ExternalCalendars.AddExternalCalendar;

/// <summary>Imports a calendar by URL for the caller. The address is checked and the calendar downloaded at once.</summary>
public sealed record AddExternalCalendarCommand(Guid HouseholdId, string? Url, string? Name, string? Color, ExternalCalendarVisibility Visibility)
    : ICommand<ExternalCalendarView>, IHouseholdRequest
{
    public string Permission => TodoPlugin.Write;
}
