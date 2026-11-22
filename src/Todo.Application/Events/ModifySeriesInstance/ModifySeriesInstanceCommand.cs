using HomeApps.BuildingBlocks.Application.Messaging;
using HomeApps.Plugins;

namespace Todo.Application.Events.ModifySeriesInstance;

/// <summary>
/// Changes the occurrence of a series on <paramref name="Date"/> (its local day): only it (<c>ThisInstance</c>, title, times
/// and reminders), it and the later ones (<c>ThisAndFuture</c>) or all (<c>All</c>). Null fields keep the current value;
/// an unchanged <c>Rrule</c> is null. Returns the changed occurrence (for ThisAndFuture the first of the new series).
/// </summary>
public sealed record ModifySeriesInstanceCommand(
    Guid HouseholdId,
    Guid SeriesId,
    DateOnly Date,
    string? Scope,
    string? Title,
    string? Description,
    string? Location,
    DateTimeOffset? StartUtc,
    DateTimeOffset? EndUtc,
    IReadOnlyList<int>? ReminderMinutes,
    string? Rrule) : ICommand<CalendarEventDetailView>, IHouseholdRequest
{
    public string Permission => TodoPlugin.Write;
}
