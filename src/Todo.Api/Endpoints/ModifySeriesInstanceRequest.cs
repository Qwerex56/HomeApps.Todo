using Todo.Application.Events.ModifySeriesInstance;

namespace Todo.Api.Endpoints;

/// <summary>Body of <c>PUT /calendar/events/{seriesId}/instances/{date}</c>; <c>Scope</c> is ThisInstance, ThisAndFuture or All. Null fields keep the current value.</summary>
internal sealed record ModifySeriesInstanceRequest(
    string? Scope,
    string? Title,
    string? Description,
    string? Location,
    DateTimeOffset? StartUtc,
    DateTimeOffset? EndUtc,
    IReadOnlyList<int>? ReminderMinutes,
    string? Rrule)
{
    public ModifySeriesInstanceCommand ToCommand(Guid householdId, Guid seriesId, DateOnly date) =>
        new(householdId, seriesId, date, Scope, Title, Description, Location, StartUtc, EndUtc, ReminderMinutes, Rrule);
}
