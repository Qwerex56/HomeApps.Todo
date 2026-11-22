using Todo.Application.ExternalCalendars.AddExternalCalendar;
using Todo.Domain.Events;

namespace Todo.Api.Endpoints;

/// <summary>Body of <c>POST /calendar/external</c>.</summary>
internal sealed record AddExternalCalendarRequest(string? Url, string? Name, string? Color, ExternalCalendarVisibility Visibility)
{
    public AddExternalCalendarCommand ToCommand(Guid householdId) => new(householdId, Url, Name, Color, Visibility);
}
